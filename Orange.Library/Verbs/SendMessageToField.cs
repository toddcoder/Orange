using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Runtime;

namespace Orange.Library.Verbs
{
   public class SendMessageToField : SendMessage, IStatement
   {
      protected string fieldName;
      protected VerbPresidenceType presidenceType;
      protected string result;
      protected string typeName;

      public SendMessageToField(string fieldName, string message, Arguments arguments, VerbPresidenceType presidenceType,
         bool inPlace = false, bool registerCall = false, bool optional = false)
         : base(message, arguments, inPlace, registerCall, optional)
      {
         this.fieldName = fieldName;
         this.presidenceType = presidenceType;
         result = "";
         typeName = "";
      }

      public override Value Evaluate()
      {
         var variable = new Variable(fieldName);
         State.Stack.Push(variable);
         var value = base.Evaluate();
         result = variable.Value.ToString();
         typeName = variable.Value.Type.ToString();
         return value;
      }

      public override VerbPresidenceType Presidence => presidenceType;

      public string Result => result;

      public string TypeName => typeName;

      public int Index { get; set; }
   }
}