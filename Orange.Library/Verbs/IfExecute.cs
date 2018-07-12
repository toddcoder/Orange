using System.Collections.Generic;
using System.Linq;
using Orange.Library.Values;
using Standard.Types.Maybe;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Values.Ignore;
using static Orange.Library.Values.Nil;
using static Standard.Types.Maybe.MaybeFunctions;
using Array = Orange.Library.Values.Array;

namespace Orange.Library.Verbs
{
   public class IfExecute : Verb, IReplaceBlocks, INSGeneratorSource, IStatement
   {
      public class IfGenerator : NSGenerator
      {
         enum IfStage
         {
            Condition,
            Result,
            ElseIf,
            Else
         }

         Block condition;
         Block.BlockGenerator resultGenerator;
         IMaybe<IfGenerator> elseIf;
         IMaybe<Block.BlockGenerator> elseBlockGenerator;
         IfStage ifStage;

         public IfGenerator(IfExecute ifExecute)
            : base(ifExecute)
         {
            var _if = ifExecute._if;
            condition = _if.Condition;
            resultGenerator = new Block.BlockGenerator(_if.Result);
            elseIf = when(_if.Next != null, () => new IfGenerator(new IfExecute(_if.Next)));
            elseBlockGenerator = when(_if.ElseBlock != null, () => new Block.BlockGenerator(_if.ElseBlock));
         }

         public override void Reset()
         {
            base.Reset();
            resultGenerator.Reset();
            if (elseIf.If(out var g))
               g.Reset();
            if (elseBlockGenerator.If(out var bg))
               bg.Reset();
            ifStage = IfStage.Condition;
         }

         public override Value Next()
         {
            using (var popper = new RegionPopper(region, "if-generator"))
            {
               popper.Push();

               switch (ifStage)
               {
                  case IfStage.Condition:
                     if (condition.IsTrue)
                     {
                        ifStage = IfStage.Result;
                        return resultGenerator.Next();
                     }

                     if (elseIf.IsSome)
                     {
                        ifStage = IfStage.ElseIf;
                        return elseIf.Value.Next();
                     }

                     if (elseBlockGenerator.IsSome)
                     {
                        ifStage = IfStage.Else;
                        return elseBlockGenerator.Value.Next();
                     }

                     return NilValue;
                  case IfStage.Result:
                     return resultGenerator.Next();
                  case IfStage.ElseIf:
                     return elseIf.Value.Next();
                  case IfStage.Else:
                     return elseBlockGenerator.Value.Next();
                  default:
                     return IgnoreValue;
               }
            }
         }
      }

      Values.If _if;
      VerbPresidenceType presidence;

      public IfExecute(Values.If _if, VerbPresidenceType presidence = VerbPresidenceType.Statement)
      {
         this._if = _if;
         this.presidence = presidence;
      }

      public override Value Evaluate() => _if.Invoke();

      public override VerbPresidenceType Presidence => presidence;

      public override string ToString() => _if.ToString();

      public IEnumerable<Block> Blocks
      {
         get
         {
            yield return _if.Condition;
            yield return _if.Result;

            if (_if.ElseBlock != null)
               yield return _if.ElseBlock;
         }
         set
         {
            var blocks = value.ToArray();
            _if.Condition = blocks[0];
            _if.Result = blocks[1];
            if (blocks.Length > 2)
               _if.ElseBlock = blocks[2];
         }
      }

      public override bool Yielding => _if.IsGeneratorAvailable;

      public INSGenerator GetGenerator() => new IfGenerator(this);

      public Value Next(int index) => null;

      public bool IsGeneratorAvailable => _if.IsGeneratorAvailable;

      public Array ToArray() => Runtime.ToArray(GetGenerator());

      public string Result => ((IStatementResult)_if).Result;

      public string TypeName => ((IStatementResult)_if).TypeName;

      public int Index { get; set; }
   }
}