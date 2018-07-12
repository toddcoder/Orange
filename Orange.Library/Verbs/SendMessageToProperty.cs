using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Managers.RegionManager;
using static Orange.Library.Runtime;

namespace Orange.Library.Verbs
{
   public class SendMessageToProperty : Verb, IStatement
   {
      string fieldName;
      string propertyName;
      Arguments propertyArguments;
      string messageName;
      Arguments messageArguments;
      VerbPresidenceType presidenceType;
      string result;
      string typeName;

      public SendMessageToProperty(string fieldName, string propertyName, Arguments propertyArguments,
         string messageName, Arguments messageArguments, VerbPresidenceType presidenceType)
      {
         this.fieldName = fieldName;
         this.propertyName = propertyName;
         this.propertyArguments = propertyArguments;
         this.messageName = messageName;
         this.messageArguments = messageArguments;
         this.presidenceType = presidenceType;
         result = "";
         typeName = "";
      }

      public override Value Evaluate()
      {
         var value = Regions[fieldName];
         var value1 = SendMessage(value, propertyName, propertyArguments);
         var value2 = SendMessage(value1, messageName, messageArguments);
         result = value2.ToString();
         typeName = value2.Type.ToString();
         return value2;
      }

      public override VerbPresidenceType Presidence => presidenceType;

      public override string ToString() => $"{fieldName}.{propertyName}.{messageName}";

      public string Result => result;

      public string TypeName => typeName;

      public int Index { get; set; }
   }
}