using System.IO;
using Orange.Library.Managers;
using Orange.Library.Verbs;
using Standard.Types.Maybe;
using static Orange.Library.Values.Null;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Orange.Library.Values
{
   public class If : Value, IStatementResult
   {
      Block condition;
      Block result;
      If next;
      Block elseBlock;

      public If(Block condition, Block result)
      {
         this.condition = condition;
         this.result = result;
         elseBlock = null;
      }

      public If()
      {
         condition = new Block();
         result = new Block();
      }

      public If Next
      {
         get => next;
         set => next = value;
      }

      public Block ElseBlock
      {
         get => elseBlock;
         set => elseBlock = value;
      }

      public override int Compare(Value value) => 0;

      public override string Text
      {
         get { return ""; }
         set { }
      }

      public override double Number { get; set; }

      public override ValueType Type => ValueType.If;

      public override bool IsTrue => Invoke().IsTrue;

      public IMaybe<CaseExecute> Case { get; set; } = none<CaseExecute>();

      public override Value Clone() => new If((Block)condition.Clone(), (Block)result.Clone())
      {
         Next = (If)Next?.Clone(),
         Case = Case
      };

      protected override void registerMessages(MessageManager manager)
      {
         manager.RegisterMessage(this, "invoke", v => ((If)v).Invoke());
         manager.RegisterMessage(this, "else", v => ((If)v).Else());
      }

      public Value Invoke()
      {
         var statementResult = (IStatementResult)this;
         statementResult.Result = "";
         statementResult.TypeName = "";

         for (var current = this; current != null; current = current.Next)
            if (current.condition.IsTrue)
            {
               Location = current.condition.ToString();
               var currentResult = current.result.Evaluate();
               statementResult.Result = currentResult.ToString();
               statementResult.TypeName = currentResult.Type.ToString();
               return currentResult;
            }

         if (elseBlock != null)
         {
            Location = elseBlock.ToString();
            var elseResult = elseBlock.Evaluate();
            statementResult.Result = elseResult.ToString();
            statementResult.TypeName = elseResult.Type.ToString();
            return elseResult;
         }

         statementResult.Result = "null";
         statementResult.TypeName = "Null";
         Location = "null";
         return NullValue;
      }

      public bool Build(out Block execute, out Block returnSignal)
      {
         for (var current = this; current != null; current = current.Next)
            if (current.condition.Evaluate().IsTrue)
            {
               evaluateResultBlock(current.result, out execute, out returnSignal);
               return true;
            }

         if (elseBlock != null)
         {
            evaluateResultBlock(elseBlock, out execute, out returnSignal);
            return true;
         }

         execute = null;
         returnSignal = null;
         return false;
      }

      static void evaluateResultBlock(Block block, out Block executeBlock, out Block returnBlock)
      {
         var builder = new CodeBuilder();
         executeBlock = null;
         returnBlock = null;
         var execute = true;
         foreach (var verb in block)
         {
            if (execute && verb is ReturnSignal)
            {
               builder.RemoveLastEnd();
               executeBlock = builder.Block;
               builder = new CodeBuilder();
               execute = false;
               continue;
            }

            builder.Verb(verb);
         }

         builder.RemoveLastEnd();
         if (execute)
            executeBlock = builder.Block;
         else
            returnBlock = builder.Block;
      }

      public Value Else()
      {
         elseBlock = Arguments.Block;
         return null;
      }

      public override string ToString()
      {
         using (var writer = new StringWriter())
         {
            writer.Write($"if {condition} [{result}]");
            if (elseBlock != null)
               writer.Write($" else [{elseBlock}]");
            if (next != null)
               writer.Write($"else{next}");
            return writer.ToString();
         }
      }

      public Block Condition
      {
         get => condition;
         set => condition = value;
      }

      public Block Result
      {
         get => result;
         set => result = value;
      }

      public string TypeName { get; set; }

      public bool IsGeneratorAvailable => result.Yielding || (next?.IsGeneratorAvailable ?? false) ||
         (elseBlock?.Yielding ?? false);

      public string Location { get; set; } = "";

      string IStatementResult.Result { get; set; }
   }
}