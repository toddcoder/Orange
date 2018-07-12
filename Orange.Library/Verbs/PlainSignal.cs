using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Runtime;

namespace Orange.Library.Verbs
{
   public class PlainSignal : Verb, IStatement
   {
      string type;
      string result;

      public PlainSignal(string type)
      {
         this.type = type;
         result = "";
      }

      public PlainSignal()
      {
         type = "";
         result = "";
      }

      public override Value Evaluate()
      {
         switch (type)
         {
            case "exit":
               State.ExitSignal = true;
               break;
            case "continue":
               State.SkipSignal = true;
               break;
         }

         result = type;
         return null;
      }

      public override VerbPresidenceType Presidence => VerbPresidenceType.Statement;

      public override string ToString() => type;

      public string Result => result;

      public string TypeName => "";

      public int Index { get; set; }
   }
}