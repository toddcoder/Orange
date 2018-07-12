using Orange.Library.Values;
using Standard.Types.Maybe;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Managers.RegionManager;
using static Orange.Library.Runtime;

namespace Orange.Library.Verbs
{
   public class IndexedSetter : Verb, IStatement
   {
      protected string fieldName;
      protected Block index;
      protected IMatched<Verb> verb;
      protected Block expression;
      protected string result;
      protected bool insert;
      protected string typeName;

      public IndexedSetter(string fieldName, Block index, IMatched<Verb> verb, Block expression, bool insert)
      {
         this.fieldName = fieldName;
         this.index = index;
         this.verb = verb;
         this.expression = expression;
         this.insert = insert;
         result = "";
         typeName = "";
      }

      protected virtual Value getValue() => Regions[fieldName];

      public override Value Evaluate()
      {
         var value1 = getValue();
         var arguments = new Arguments();
         arguments.AddArgument(index);
         var assignedValue = expression.AssignmentValue();
         var message = insert ? "insertSet" : SetterName("item");
         if (verb.IsMatched)
         {
            var value2 = SendMessage(value1, GetterName("item"), arguments);
            var stack = State.Stack;
            stack.Push(value2);
            stack.Push(assignedValue);
            var evaluated = verb.Value.Evaluate();
            arguments.AddArgument(evaluated);
            SendMessage(value1, message, arguments);
         }
         else
         {
            arguments.AddArgument(assignedValue);
            SendMessage(value1, message, arguments);
         }
         result = value1.ToString();
         typeName = value1.Type.ToString();
         return null;
      }

      public override VerbPresidenceType Presidence => VerbPresidenceType.Statement; //VerbPresidenceType.Setter;

      public string Result => result;

      public string TypeName => typeName;

      public int Index { get; set; }

      public override string ToString()
      {
         return $"{fieldName}[{(insert ? "+" : "")}{index}] {verb.FlatMap(v => v.ToString(), () => "")}= {expression}";
      }
   }
}