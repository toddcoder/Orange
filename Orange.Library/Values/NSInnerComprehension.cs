using System;
using Orange.Library.Managers;
using Standard.Types.Maybe;
using static Orange.Library.Runtime;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Orange.Library.Values
{
   public class NSInnerComprehension : Value, INSGenerator
   {
      Parameters parameters;
      Block generatorSource;
      NSIterator iterator;
      IMaybe<Block> ifBlock;
      int index;
      Region region;
      bool more;

      public NSInnerComprehension(Parameters parameters, Block generatorSource, IMaybe<Block> ifBlock)
      {
         this.parameters = parameters;
         this.generatorSource = generatorSource;
         this.ifBlock = ifBlock;
         iterator = null;
         index = -1;
         region = new Region();
      }

      public override int Compare(Value value) => 0;

      public override string Text { get; set; } = "";

      public override double Number { get; set; }

      public override ValueType Type => ValueType.Generator;

      public override bool IsTrue => false;

      public override Value Clone() => new NSInnerComprehension((Parameters)parameters.Clone(),
         (Block)generatorSource.Clone(), ifBlock.Map(b => ((Block)b.Clone()).Some()))
      {
         NextComprehension = NextComprehension.Map(c => ((NSInnerComprehension)c.Clone()).Some())
      };

      protected override void registerMessages(MessageManager manager)
      {
         manager.RegisterMessage(this, "reset", v => ((NSInnerComprehension)v).DoReset());
         manager.RegisterMessage(this, "next", v => ((NSInnerComprehension)v).Next());
         manager.RegisterMessage(this, "if", v => ((NSInnerComprehension)v).If());
         manager.RegisterMessage(this, "ifNot", v => ((NSInnerComprehension)v).IfNot());
         manager.RegisterMessage(this, "map", v => ((NSInnerComprehension)v).Map());
         manager.RegisterMessage(this, "mapIf", v => ((NSInnerComprehension)v).MapIf());
         manager.RegisterMessage(this, "skip", v => ((NSInnerComprehension)v).Skip());
         manager.RegisterMessage(this, "skipWhile", v => ((NSInnerComprehension)v).SkipWhile());
         manager.RegisterMessage(this, "skipUntil", v => ((NSInnerComprehension)v).SkipUntil());
         manager.RegisterMessage(this, "take", v => ((NSInnerComprehension)v).Take());
         manager.RegisterMessage(this, "takeWhile", v => ((NSInnerComprehension)v).TakeWhile());
         manager.RegisterMessage(this, "takeUntil", v => ((NSInnerComprehension)v).TakeUntil());
         manager.RegisterMessage(this, "unique", v => ((NSInnerComprehension)v).Unique());
         manager.RegisterMessage(this, "group", v => ((NSInnerComprehension)v).Group());
         manager.RegisterMessage(this, "flat", v => ((NSInnerComprehension)v).Flat());
         manager.RegisterMessage(this, "first", v => ((NSInnerComprehension)v).First());
         manager.RegisterMessage(this, "foldl", v => ((NSInnerComprehension)v).FoldL());
         manager.RegisterMessage(this, "foldr", v => ((NSInnerComprehension)v).FoldR());
         manager.RegisterMessage(this, "allOf", v => ((NSInnerComprehension)v).AllOf());
         manager.RegisterMessage(this, "anyOf", v => ((NSInnerComprehension)v).AnyOf());
         manager.RegisterMessage(this, "oneOf", v => ((NSInnerComprehension)v).OneOf());
         manager.RegisterMessage(this, "noneOf", v => ((NSInnerComprehension)v).NoneOf());
         manager.RegisterMessage(this, "split", v => ((NSInnerComprehension)v).Split());
         manager.RegisterMessage(this, "splitWhile", v => ((NSInnerComprehension)v).SplitWhile());
         manager.RegisterMessage(this, "splitUntil", v => ((NSInnerComprehension)v).SplitUntil());
         manager.RegisterMessage(this, "more", v => ((NSInnerComprehension)v).More);
      }

      public IMaybe<NSInnerComprehension> NextComprehension { get; set; } = none<NSInnerComprehension>();

      public Value DoReset()
      {
         Reset();
         return this;
      }

      public void Reset()
      {
         index = 0;
         var value = generatorSource.Evaluate().PossibleGenerator();
         Assert(value.IsSome, "Inner comprehension", "Source must be a generator");
         iterator = new NSIterator(value.Value);
         iterator.Reset();
         more = true;
      }

      Value nextGeneratorValue()
      {
         RejectNull(iterator, LOCATION, "Reset not called");
         var next = iterator.Next();
         if (next.IsNil)
         {
            more = false;
            return next;
         }

         SetValue(next);
         for (var i = 0; i < MAX_LOOP && !ifTrue() && !next.IsNil; i++)
         {
            next = iterator.Next();
            SetValue(next);
         }

         more = !next.IsNil;
         return next;
      }

      public Value Next()
      {
         Value next;
         if (NextComprehension.IsSome)
         {
            var nextComprehension = NextComprehension.Value;
            if (index == 0)
            {
               next = nextGeneratorValue();
               if (next.IsNil)
               {
                  more = false;
                  return next;
               }

               nextComprehension.Reset();
            }

            next = nextComprehension.Next();
            if (next.IsNil)
            {
               next = nextGeneratorValue();
               if (next.IsNil)
               {
                  more = false;
                  return next;
               }

               nextComprehension.Reset();
               next = nextComprehension.Next();
            }
         }
         else
            next = nextGeneratorValue();

         more = !next.IsNil;
         return next;
      }

      public Value If() => NSGenerator.If(this, Arguments);

      public Value IfNot() => NSGenerator.IfNot(this, Arguments);

      public Value Map() => NSGenerator.Map(this, Arguments);

      public Value MapIf() => NSGenerator.MapIf(this, Arguments);

      public Value Skip() => NSGenerator.Skip(this, Arguments);

      public Value SkipWhile() => NSGenerator.SkipWhile(this, Arguments);

      public Value SkipUntil() => NSGenerator.SkipUntil(this, Arguments);

      public Value Take() => NSGenerator.Take(this, Arguments);

      public Value TakeWhile() => NSGenerator.TakeWhile(this, Arguments);

      public Value TakeUntil() => NSGenerator.TakeUntil(this, Arguments);

      public Value Group() => NSGenerator.Group(this, Arguments);

      public Value Array() => GeneratorToArray(this);

      public Value FoldL() => NSGenerator.FoldL(this, Arguments);

      public Value FoldR() => NSGenerator.FoldR(this, Arguments);

      public Value AnyOf() => NSGenerator.AnyOf(this, Arguments);

      public Value AllOf() => NSGenerator.AllOf(this, Arguments);

      public Value OneOf() => NSGenerator.OneOf(this, Arguments);

      public Value NoneOf() => NSGenerator.NoneOf(this, Arguments);

      public Value Split() => NSGenerator.Split(this, Arguments);

      public Value SplitWhile() => NSGenerator.SplitWhile(this, Arguments);

      public Value SplitUntil() => NSGenerator.SplitUntil(this, Arguments);

      public Value Unique() => NSGenerator.Unique(this, Arguments);

      public Value Flat() => NSGenerator.Flat(this, Arguments);

      public Value First() => NSGenerator.First(this, Arguments);

      public Region Region
      {
         get => region;
         set => region = value.Clone();
      }

      public INSGeneratorSource GeneratorSource => generatorSource.Evaluate() is INSGeneratorSource source ? source :
         throw new ApplicationException($"{generatorSource} not a generator source");

      public void Visit(Value value) { }

      public bool More => more;

      bool ifTrue() => ifBlock.FlatMap(b => b.IsTrue, () => true);

      public void SetValue(Value value) => parameters.SetValues(value, index++);

      public override string ToString()
      {
         return $"{parameters} in {generatorSource}{ifBlock.FlatMap(b => $" if {b} ", () => "")}" +
            $"{NextComprehension.FlatMap(c => $", {c}", () => "")}";
      }
   }
}