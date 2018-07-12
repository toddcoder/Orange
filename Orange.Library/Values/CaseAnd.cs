using Orange.Library.Managers;

namespace Orange.Library.Values
{
   public class CaseAnd : Value
   {
      Case left;
      Case right;

      public CaseAnd(Case left, Case right)
      {
         this.left = left;
         this.right = right;
      }

      public override int Compare(Value value) => 0;

      public override string Text { get; set; }

      public override double Number { get; set; }

      public override ValueType Type => ValueType.CaseAnd;

      public override bool IsTrue => left.IsTrue && right.IsTrue;

      public override Value Clone() => new CaseAnd((Case)left.Clone(), (Case)right.Clone());

      public Block If { get; set; }

      protected override void registerMessages(MessageManager manager)
      {
         manager.RegisterMessage(this, "if", v => ((CaseAnd)v).SetIf());
         manager.RegisterMessage(this, "map", v => ((CaseAnd)v).Map());
      }

      public Value SetIf()
      {
         If = Arguments.Executable;
         return this;
      }

      public Value Map()
      {
         var region = new Region();
         using (var popper = new RegionPopper(region, "and"))
         {
            popper.Push();
            var block = Arguments.Executable;
            if (block.CanExecute)
            {
               var matched = Case.Match(left.Value, left.Comparisand, region, false, false, null);
               if (matched)
               {
                  matched = Case.Match(right.Value, right.Comparisand, region, false, false, null);
                  if (matched)
                  {
                     if (If != null && !If.Evaluate().IsTrue)
                     {
                        left.Matched = false;
                        return new Nil();
                     }

                     var result = block.Evaluate();
                     return result;
                  }

                  left.Matched = false;
                  return new Nil();
               }

               left.Matched = false;
               return new Nil();
            }

            return this;
         }
      }

      public override string ToString() => left + " && " + right;
   }
}