using Orange.Library.Managers;
using static Orange.Library.Runtime;

namespace Orange.Library.Values
{
   public class ListSequence : Value, ISequenceSource
   {
      List list;
      List head;

      public ListSequence(List list) => this.list = head = list;

      public ListSequence() => list = head = new List();

      public override int Compare(Value value) => 0;

      public override string Text
      {
         get;
         set;
      } = "";

      public override double Number
      {
         get;
         set;
      }

      public override ValueType Type => ValueType.Sequence;

      public override bool IsTrue => false;

      public override Value Clone() => new ListSequence((List)list.Clone());

      protected override void registerMessages(MessageManager manager)
      {
         manager.RegisterMessage(this, "map", v => ((ListSequence)v).Map());
         manager.RegisterMessage(this, "if", v => ((ListSequence)v).If());
         manager.RegisterMessage(this, "unless", v => ((ListSequence)v).Unless());
         manager.RegisterMessage(this, "take", v => ((ListSequence)v).Take());
         manager.RegisterMessage(this, "next", v => ((ListSequence)v).Next());
         manager.RegisterMessage(this, "reset", v => ((ListSequence)v).Reset());
         manager.RegisterMessage(this, "arr", v => ((ListSequence)v).Array);
      }

      public Value Map() => new Sequence(this)
      {
         Arguments = Arguments.Clone()
      }.Map();

      public Value If() => new Sequence(this)
      {
         Arguments = Arguments.Clone()
      }.If();

      public Value Unless() => new Sequence(this)
      {
         Arguments = Arguments.Clone()
      }.Unless();

      public Value Take() => new Sequence(this)
      {
         Arguments = Arguments.Clone()
      }.Take();

      public Value Next()
      {
         list = list.Tail;
         if (list.IsEmpty)
         {
            Reset();
            return Nil.NilValue;
         }
         return list.Head;
      }

      public ISequenceSource Copy() => (ISequenceSource)Clone();

      public Value Reset()
      {
         list = head;
         return this;
      }

      public int Limit => MAX_ARRAY;

      public Array Array => Array.ArrayFromSequence(this);

   }
}