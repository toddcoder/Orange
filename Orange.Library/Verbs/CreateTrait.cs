using System.Collections.Generic;
using System.Linq;
using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Managers.RegionManager;

namespace Orange.Library.Verbs
{
   public class CreateTrait : Verb, IStatement
   {
      Trait trait;
      List<string> traitNames;
      string result;

      public CreateTrait(Trait trait, List<string> traitNames)
      {
         this.trait = trait;
         this.traitNames = traitNames;
         result = "";
      }

      public CreateTrait()
         : this(null, new List<string>()) { }

      public override Value Evaluate()
      {
         if (traitNames != null)
            foreach (var item in traitNames
               .Select(name => Regions[name]).OfType<Trait>()
               .SelectMany(parentTrait => parentTrait.Members
                  .Where(item => !trait.Members.ContainsKey(item.Key))))
               trait.Members[item.Key] = item.Value;

         Regions.CreateVariable(trait.Name);
         Regions[trait.Name] = trait;
         result = trait.ToString();

         return null;
      }

      public override VerbPresidenceType Presidence => VerbPresidenceType.Statement;

      public string Result => result;

      public string TypeName => "Trait";

      public int Index { get; set; }
   }
}