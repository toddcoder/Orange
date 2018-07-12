using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Managers.RegionManager;
using static Orange.Library.Runtime;

namespace Orange.Library.Verbs
{
   public class PushIndexer : Verb
   {
      const string LOCATION = "Push indexer";

      Block indexes;

      public PushIndexer(Block indexes) => this.indexes = indexes;

      public PushIndexer()
         : this(null) { }

      public Block Indexes => indexes;

      public override Value Evaluate()
      {
         var value = State.Stack.Pop(false, LOCATION);
         if (value is KeyIndexer keyIndexer)
            value = keyIndexer;
         var isVariable = value.IsVariable;
         Variable variable = null;
         if (isVariable)
         {
            variable = (Variable)value;
            if (Regions.VariableExists(variable.Name) || value is ObjectVariable)
               value = variable.Value;
            else
            {
               value = new Array();
               variable.Value = value;
            }
         }
         if (value.IsArray)
            return new ChooseIndexer((Array)value.SourceArray, indexes);

         switch (value)
         {
            case String str:
               return new StringIndexer(str, indexes);
            case Object obj:
               return new ObjectIndexer(obj, indexes);
            case Class cls:
               var arguments = new Arguments(indexes);
               return Invoke.Evaluate(cls, arguments);
            case XMLElement xml:
               return new XMLElementIndexer(xml, indexes.Evaluate().Text);
            case List list:
               return new ListIndexer(list, indexes);
         }

         value = new Array();
         if (isVariable)
            variable.Value = value;
         return new ChooseIndexer((Array)value, indexes);
      }

      public override string ToString() => $"[{indexes}]";

      public override VerbPresidenceType Presidence => VerbPresidenceType.Indexer;
   }
}