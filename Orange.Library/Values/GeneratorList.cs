using System.Collections.Generic;
using Orange.Library.Managers;
using static Orange.Library.Runtime;
using static Orange.Library.Values.Nil;

namespace Orange.Library.Values
{
   public class GeneratorList : Value, INSGenerator
   {
      List<INSGenerator> generators;
      int index;
      bool more;

      public GeneratorList()
      {
         generators = new List<INSGenerator>();
         index = 0;
      }

      public GeneratorList(IEnumerable<INSGenerator> generators)
         : this()
      {
         this.generators.AddRange(generators);
      }

      public void Add(INSGenerator generator) => generators.Add(generator);

      public override int Compare(Value value) => 0;

      public override string Text { get; set; } = "";

      public override double Number { get; set; }

      public override ValueType Type => ValueType.Generator;

      public override bool IsTrue => generators.Count > 0;

      public override Value Clone() => new GeneratorList(generators);

      protected override void registerMessages(MessageManager manager)
      {
         manager.RegisterMessage(this, "reset", v => ((GeneratorList)v).DoReset());
         manager.RegisterMessage(this, "next", v => ((INSGenerator)v).Next());
         manager.RegisterMessage(this, "if", v => ((INSGenerator)v).If());
         manager.RegisterMessage(this, "ifNot", v => ((INSGenerator)v).IfNot());
         manager.RegisterMessage(this, "map", v => ((INSGenerator)v).Map());
         manager.RegisterMessage(this, "mapIf", v => ((INSGenerator)v).MapIf());
         manager.RegisterMessage(this, "skip", v => ((INSGenerator)v).Skip());
         manager.RegisterMessage(this, "skipWhile", v => ((INSGenerator)v).SkipWhile());
         manager.RegisterMessage(this, "skipUntil", v => ((INSGenerator)v).SkipUntil());
         manager.RegisterMessage(this, "take", v => ((INSGenerator)v).Take());
         manager.RegisterMessage(this, "takeWhile", v => ((INSGenerator)v).TakeWhile());
         manager.RegisterMessage(this, "takeUntil", v => ((INSGenerator)v).TakeUntil());
         manager.RegisterMessage(this, "unique", v => ((INSGenerator)v).Unique());
         manager.RegisterMessage(this, "flat", v => ((INSGenerator)v).Flat());
         manager.RegisterMessage(this, "first", v => ((INSGenerator)v).First());
         manager.RegisterMessage(this, "group", v => ((INSGenerator)v).Group());
         manager.RegisterMessage(this, "array", v => ((INSGenerator)v).Array());
         manager.RegisterMessage(this, "foldl", v => ((INSGenerator)v).FoldL());
         manager.RegisterMessage(this, "foldr", v => ((INSGenerator)v).FoldR());
         manager.RegisterMessage(this, "allOf", v => ((INSGenerator)v).AllOf());
         manager.RegisterMessage(this, "anyOf", v => ((INSGenerator)v).AnyOf());
         manager.RegisterMessage(this, "oneOf", v => ((INSGenerator)v).OneOf());
         manager.RegisterMessage(this, "noneOf", v => ((INSGenerator)v).NoneOf());
         manager.RegisterMessage(this, "split", v => ((INSGenerator)v).Split());
         manager.RegisterMessage(this, "splitWhile", v => ((INSGenerator)v).SplitWhile());
         manager.RegisterMessage(this, "splitUntil", v => ((INSGenerator)v).SplitUntil());
         manager.RegisterMessage(this, "concat", v => ((GeneratorList)v).Concatenate());
         manager.RegisterMessage(this, "more", v => ((GeneratorList)v).More);
      }

      public Value DoReset()
      {
         Reset();
         return this;
      }

      public void Reset()
      {
         foreach (var generator in generators)
         {
            generator.Region = Region;
            generator.Reset();
         }
         index = 0;
         more = true;
      }

      public Value Next()
      {
         if (index >= generators.Count)
         {
            more = false;
            return NilValue;
         }
         for (var i = 0; i < MAX_LOOP && index < generators.Count; i++)
         {
            var generator = generators[index];
            var value = generator.Next();
            if (value.IsNil)
            {
               index++;
               continue;
            }

            more = !value.IsNil;
            return value;
         }

         more = false;
         return NilValue;
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

      public Value Array() => ToArray(this);

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

      public Value Concatenate()
      {
         var generator = Arguments[0].PossibleGenerator();
         if (generator.IsSome)
            Add(generator.Value);
         return this;
      }

      public Region Region { get; set; } = new Region();

      public INSGeneratorSource GeneratorSource => null;

      public void Visit(Value value) { }

      public bool More => more;

      public override string ToString() => Array().ToString();
   }
}