using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Runtime;

namespace Orange.Library.Verbs
{
   public class Assert : Verb, IStatement
   {
      const string LOCATION = "Assert";
      bool assert;
      Block condition;
      String message;
      string result;

      public Assert(bool assert, Block condition, String message)
      {
         this.assert = assert;

         this.condition = condition;
         this.condition.AutoRegister = false;

         this.message = message;
         result = "";
      }

      public override Value Evaluate()
      {
         if (assert)
         {
            result = "assertion true";
            Assert(condition.IsTrue, LOCATION, message.Text);
         }
         else
         {
            result = "not rejected";
            Reject(condition.IsTrue, LOCATION, message.Text);
         }
         return null;
      }

      public override VerbPresidenceType Presidence => VerbPresidenceType.Push;

      public override string ToString() => $"{(assert ? "assert" : "reject")} {condition} then '{message}'";

      public string Result => result;

      public string TypeName => "";

      public int Index { get; set; }
   }
}