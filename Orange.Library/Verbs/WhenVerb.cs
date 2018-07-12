using Orange.Library.Values;
using Standard.Types.Maybe;
using static Orange.Library.Managers.ExpressionManager;

namespace Orange.Library.Verbs
{
   public class WhenVerb : Verb, IStatement
   {
      protected Verb verb;
      protected Block condition;
      protected IMaybe<IStatement> statement;
      protected string result;
      protected string typeName;

      public WhenVerb(Verb verb, Block condition)
      {
         this.verb = verb;
         this.condition = condition;
         statement = this.verb.IfCast<IStatement>();
         result = "";
         typeName = "";
      }

      protected virtual bool isTrue() => condition.IsTrue;

      public override Value Evaluate()
      {
         if (isTrue())
         {
            var verbResult = verb.Evaluate();
            result = statement.FlatMap(s => s.Result, () => "");
            typeName = statement.FlatMap(s => s.TypeName, () => "");
            Index = statement.FlatMap(s => s.Index, () => 0);
            return verbResult;
         }

         return null;
      }

      public override VerbPresidenceType Presidence => VerbPresidenceType.Statement;

      public string Result => result;

      public string TypeName => typeName;

      public int Index { get; set; }

      public override string ToString() => $"{verb} when {condition}";
   }
}