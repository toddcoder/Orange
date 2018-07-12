using System;
using Orange.Library.Managers;

namespace Orange.Library.Values
{
   public class NSGeneratorSource : Value, INSGeneratorSource
   {
      Value source;
      Func<int, Value> next;

      public NSGeneratorSource(Value source, Func<int, Value> next)
      {
         this.source = source;
         this.next = next;
      }

      public override int Compare(Value value) => source.Compare(value);

      public override string Text
      {
         get { return source.Text; }
         set { }
      }

      public override double Number
      {
         get { return source.Number; }
         set { }
      }

      public override ValueType Type => ValueType.GeneratorSource;

      public override bool IsTrue => source.IsTrue;

      public override Value Clone() => new NSGeneratorSource(source.Clone(), next);

      protected override void registerMessages(MessageManager manager) { }

      public INSGenerator GetGenerator() => new NSGenerator(this);

      public Value Next(int index) => next(index);

      public bool IsGeneratorAvailable => true;

      public Array ToArray() => (Array)GetGenerator().Array();

      public override string ToString() => $"!{source}";
   }
}