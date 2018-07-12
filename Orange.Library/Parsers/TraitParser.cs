using System.Collections.Generic;
using Orange.Library.Parsers.Line;
using Orange.Library.Values;
using Orange.Library.Verbs;
using Standard.Types.Collections;
using static Orange.Library.Compiler;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Runtime;

namespace Orange.Library.Parsers
{
   public class TraitParser : Parser
   {
      TraitBodyParser bodyParser;
      DoesParser doesParser;
      EndOfLineParser endOfLineParser;
      List<string> traits;

      public TraitParser()
         : base($"^ /('trait' /s+) /({REGEX_VARIABLE})")
      {
         bodyParser = new TraitBodyParser();
         doesParser = new DoesParser();
         endOfLineParser = new EndOfLineParser();
         traits = new List<string>();
      }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, tokens[1].Length, KeyWords);
         var name = tokens[2];
         Color(name.Length, Types);
         var index = NextPosition;
         if (doesParser.Scan(source, index))
         {
            traits = doesParser.Traits;
            index = doesParser.Position;
         }
         if (endOfLineParser.Scan(source, index))
            index = endOfLineParser.Position;
         var members = new Hash<string, Value>();
         InClassDefinition = true;
         foreach (var traitName in traits)
         {
            var trait = CompilerState.Trait(traitName);
            Assert(trait.IsSome, LOCATION, $"Trait {traitName} isn't defined");
            foreach (var item in trait.Value.Members)
               members[item.Key] = item.Value;
         }

         try
         {
            AdvanceTabs();
            while (index < source.Length)
               if (bodyParser.Scan(source, index) && bodyParser.Parser.IsSome)
               {
                  var parser = bodyParser.Parser.Value;
                  index = parser.Position;
                  if (parser is ITraitName aTraitName)
                  {
                     var traitName = aTraitName;
                     var memberName = traitName.MemberName;
                     var value = parser.Result.Value;
                     if (value != null)
                        members[memberName] = value;
                     else
                     {
                        value = traitName.Getter;
                        if (value != null)
                           members[GetterName(memberName)] = value;
                        value = traitName.Setter;
                        if (value != null)
                           members[SetterName(memberName)] = value;
                        members[MangledName(memberName)] = new Null();
                     }
                  }
                  else
                     return null;
               }
               else
                  break;
         }
         finally
         {
            InClassDefinition = false;
            RegressTabs();
         }

         var newTrait = new Trait(name, members);

         overridePosition = index;

         CompilerState.RegisterTrait(newTrait);

         return new CreateTrait(newTrait, traits) { Index = position };
      }

      public override string VerboseName => "trait";
   }
}