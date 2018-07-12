using System.Collections.Generic;
using Orange.Library.Managers;
using Orange.Library.Verbs;

namespace Orange.Library.Values
{
   public class ArrayYielder : Value, ISequenceSource
   {
      List<Lambda> closures;
      int index;

      public ArrayYielder()
      {
         closures = new List<Lambda>();
         index = -1;
      }

      public ArrayYielder(List<Lambda> closures)
      {
         this.closures = closures;
         index = -1;
      }

      public void Add(Value value)
      {
         if (value is Lambda lambda)
            closures.Add(lambda);
         var newBlock = new Block { new Push(value) };
         var block = value as Block ?? newBlock;
         closures.Add(new Lambda(RegionManager.Regions.Current.Clone(), block, new NullParameters(), true));
      }

      public override int Compare(Value value) => 0;

      public override string Text { get; set; }

      public override double Number { get; set; }

      public override ValueType Type => ValueType.ArrayYielder;

      public override bool IsTrue => false;

      public override Value Clone() => new ArrayYielder(closures);

      protected override void registerMessages(MessageManager manager)
      {
         manager.RegisterMessage(this, "map", v => ((ArrayYielder)v).Map());
         manager.RegisterMessage(this, "if", v => ((ArrayYielder)v).If());
         manager.RegisterMessage(this, "unless", v => ((ArrayYielder)v).Unless());
         manager.RegisterMessage(this, "take", v => ((ArrayYielder)v).Take());
         manager.RegisterMessage(this, "next", v => ((ArrayYielder)v).Next());
         manager.RegisterMessage(this, "reset", v => ((ArrayYielder)v).Reset());
         manager.RegisterMessage(this, "peek", v => ((ArrayYielder)v).Peek());
      }

      public Value Map()
      {
         var sequence = new Sequence(this) { Arguments = Arguments.Clone() };
         return sequence.Map();
      }

      public Value If()
      {
         var sequence = new Sequence(this) { Arguments = Arguments.Clone() };
         return sequence.If();
      }

      public Value Unless()
      {
         var sequence = new Sequence(this) { Arguments = Arguments.Clone() };
         return sequence.Unless();
      }

      public Value Take()
      {
         var sequence = new Sequence(this) { Arguments = Arguments.Clone() };
         return sequence.Take();
      }

      public Value Next() => ++index < closures.Count ? closures[index].Evaluate(new Arguments()) : new Nil();

      public ISequenceSource Copy() => (ISequenceSource)Clone();

      public Value Reset()
      {
         index = -1;
         return this;
      }

      public int Limit => closures.Count;

      public Array Array => Array.ArrayFromSequence(this);

      public bool HasYielded => closures.Count > 0;

      public Sequence Sequence() => new Sequence(this);

      public override Value AlternateValue(string message) => Sequence();

      public override Value AssignmentValue() => Sequence();

      public override Value ArgumentValue() => Sequence();

      public Value Peek()
      {
         var nextIndex = index + 1;
         return nextIndex < closures.Count ? closures[nextIndex].Evaluate(new Arguments()) : new Nil();
      }

      public override string ToString() => $"{{{Peek()}}}.yield";
   }
}