using System.Collections.Generic;
using Orange.Library.Junctions;
using Orange.Library.Managers;
using Standard.Types.Collections;
using static Orange.Library.ParameterAssistant;
using static Orange.Library.ParameterAssistant.SignalType;
using static Orange.Library.Runtime;
using static Orange.Library.Values.Nil;
using static Orange.Library.Values.NSGeneratorMod;

namespace Orange.Library.Values
{
   public class NSGenerator : Value, INSGenerator, ISharedRegion
   {
      public static Value Reset(INSGenerator generator)
      {
         generator.Reset();
         return (Value)generator;
      }

      public static Value If(INSGenerator generator, Arguments arguments) => new NSGeneratorMod(generator, new IfModifier(arguments));

      public static Value IfNot(INSGenerator generator, Arguments arguments) => new NSGeneratorMod(generator, new IfNotModifier(arguments));

      public static Value Map(INSGenerator generator, Arguments arguments) => new NSGeneratorMod(generator, new MapModifier(arguments));

      public static Value MapIf(INSGenerator generator, Arguments arguments) => new NSGeneratorMod(generator, new MapIfModifier(arguments));

      public static Value Skip(INSGenerator generator, Arguments arguments) => new NSGeneratorMod(generator, new SkipModifier(arguments));

      public static Value SkipWhile(INSGenerator generator, Arguments arguments) => new NSGeneratorMod(generator, new SkipWhileModifier(arguments));

      public static Value SkipUntil(INSGenerator generator, Arguments arguments) => new NSGeneratorMod(generator, new SkipUntilModifier(arguments));

      public static Value Take(INSGenerator generator, Arguments arguments) => new NSGeneratorMod(generator, new TakeModifier(arguments));

      public static Value TakeWhile(INSGenerator generator, Arguments arguments) => new NSGeneratorMod(generator, new TakeWhileModifier(arguments));

      public static Value TakeUntil(INSGenerator generator, Arguments arguments) => new NSGeneratorMod(generator, new TakeUntilModifier(arguments));

      public static Value Flat(INSGenerator generator, Arguments arguments) => new FlatGenerator(generator);

      public static Value Group(INSGenerator generator, Arguments arguments)
      {
         var count = arguments[0].Int;
         if (count > 0)
         {
            var iterator = new NSIterator(generator);
            var array = new Array();
            iterator.Reset();
            var value = iterator.Next();
            for (var i = 0; i < MAX_ARRAY && !value.IsNil; i += count)
            {
               var subArray = new Array();
               for (var j = 0; j < count && !value.IsNil; j++)
               {
                  subArray.Add(value);
                  value = iterator.Next();
               }

               array.Add(value);
               value = iterator.Next();
            }

            return array;
         }

         using (var assistant = new ParameterAssistant(arguments))
         {
            var block = assistant.Block();
            if (block == null)
               return ToArray(generator);

            var hash = new AutoHash<string, List<Value>>
            {
               Default = DefaultType.Lambda,
               DefaultLambda = k => new List<Value>(),
               AutoAddDefault = true
            };

            assistant.IteratorParameter();
            var iterator = new NSIterator(generator);
            iterator.Reset();
            foreach (var item in iterator)
            {
               assistant.SetIteratorParameter(item);
               var key = block.Evaluate().Text;
               hash[key].Add(item);
            }

            var array = new Array();
            foreach (var item in hash)
               array[item.Key] = new Array(item.Value);

            return array;
         }
      }

      public static Value FoldL(INSGenerator generator, Arguments arguments)
      {
         var iterator = new NSIterator(generator);
         using (var assistant = new ParameterAssistant(arguments))
         {
            var block = assistant.Block();
            if (block == null)
               return NilValue;

            iterator.Reset();

            assistant.TwoValueParameters();
            var initialFromArguments = arguments[0];
            var initialValue = initialFromArguments.IsEmpty ? iterator.Next() : initialFromArguments;
            if (initialValue.IsNil)
               return initialValue;

            var secondValue = iterator.Next();
            if (secondValue.IsNil)
               return initialValue;

            assistant.SetParameterValues(initialValue, secondValue);
            var value = block.Evaluate();
            var signal = Signal();
            if (signal == Breaking)
               return value;

            switch (signal)
            {
               case ReturningNull:
                  return null;
               case Continuing:
                  return value;
            }

            var next = iterator.Next();
            if (next.IsNil)
               return value;

            for (var i = 0; i < MAX_LOOP; i++)
            {
               assistant.SetParameterValues(value, next);
               value = block.Evaluate();
               signal = Signal();
               if (signal == Breaking)
                  break;

               switch (signal)
               {
                  case ReturningNull:
                     return null;
                  case Continuing:
                     continue;
               }

               next = iterator.Next();
               if (next.IsNil)
                  return value;
            }

            return value;
         }
      }

      public static Value FoldR(INSGenerator generator, Arguments arguments)
      {
         var iterator = new NSIterator(new NSStackGenerator(generator.GeneratorSource));

         using (var assistant = new ParameterAssistant(arguments))
         {
            var block = assistant.Block();
            if (block == null)
               return NilValue;

            iterator.Reset();

            assistant.TwoValueParameters();
            var initialFromArguments = arguments[0];
            iterator.Reset();
            var initialValue = initialFromArguments.IsEmpty ? iterator.Next() : initialFromArguments;
            if (initialValue.IsNil)
               return initialValue;

            var secondValue = iterator.Next();
            if (secondValue.IsNil)
               return initialValue;

            assistant.SetParameterValues(secondValue, initialValue);
            var value = block.Evaluate();
            var signal = Signal();
            if (signal == Breaking)
               return value;

            switch (signal)
            {
               case ReturningNull:
                  return null;
               case Continuing:
                  return value;
            }

            var next = iterator.Next();
            if (next.IsNil)
               return value;

            for (var i = 0; i < MAX_LOOP; i++)
            {
               assistant.SetParameterValues(next, value);
               value = block.Evaluate();
               signal = Signal();
               if (signal == Breaking)
                  break;

               switch (signal)
               {
                  case ReturningNull:
                     return null;
                  case Continuing:
                     continue;
               }

               next = iterator.Next();
               if (next.IsNil)
                  return value;
            }

            return value;
         }
      }

      public static Value AnyOf(INSGenerator generator, Arguments arguments)
      {
         var junction = new AnyJunction(generator, arguments);
         return junction.Evaluate();
      }

      public static Value AllOf(INSGenerator generator, Arguments arguments)
      {
         var junction = new AllJunction(generator, arguments);
         return junction.Evaluate();
      }

      public static Value OneOf(INSGenerator generator, Arguments arguments)
      {
         var junction = new OneJunction(generator, arguments);
         return junction.Evaluate();
      }

      public static Value NoneOf(INSGenerator generator, Arguments arguments)
      {
         var junction = new NoneJunction(generator, arguments);
         return junction.Evaluate();
      }

      public static Value Unique(INSGenerator generator, Arguments arguments) => new NSGeneratorMod(generator, new UniqueModifier(arguments));

      public static Value First(INSGenerator generator, Arguments arguments)
      {
         var iterator = new NSIterator(generator);
         iterator.Reset();
         var next = iterator.Next();
         if (next.IsNil)
            return new None();

         return new Some(next);
      }

      protected INSGeneratorSource generatorSource;
      protected int index;
      protected Region region;
      protected Region sharedRegion;
      protected bool more;

      public NSGenerator(INSGeneratorSource generatorSource)
      {
         this.generatorSource = generatorSource;
         index = -1;
         region = new Region();
      }

      public INSGeneratorSource GeneratorSource => generatorSource;

      public virtual void Visit(Value value) { }

      public bool More => more;

      public override int Compare(Value value) => 0;

      public override string Text { get; set; } = "";

      public override double Number { get; set; }

      public override ValueType Type => ValueType.Generator;

      public override bool IsTrue => false;

      public override Value Clone() => new NSGenerator(generatorSource);

      protected override void registerMessages(MessageManager manager)
      {
         manager.RegisterMessage(this, "reset", v => ((INSGenerator)v).DoReset());
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
         manager.RegisterMessage(this, "group", v => ((INSGenerator)v).Group());
         manager.RegisterMessage(this, "array", v => ((INSGenerator)v).Array());
         manager.RegisterMessage(this, "split", v => ((INSGenerator)v).Split());
         manager.RegisterMessage(this, "splitWhile", v => ((INSGenerator)v).SplitWhile());
         manager.RegisterMessage(this, "splitUntil", v => ((INSGenerator)v).SplitUntil());
         manager.RegisterMessage(this, "unique", v => ((INSGenerator)v).Unique());
         manager.RegisterMessage(this, "flat", v => ((INSGenerator)v).Flat());
         manager.RegisterMessage(this, "first", v => ((INSGenerator)v).First());
         manager.RegisterMessage(this, "foldl", v => ((INSGenerator)v).FoldL());
         manager.RegisterMessage(this, "foldr", v => ((INSGenerator)v).FoldR());
         manager.RegisterMessage(this, "allOf", v => ((INSGenerator)v).AllOf());
         manager.RegisterMessage(this, "anyOf", v => ((INSGenerator)v).AnyOf());
         manager.RegisterMessage(this, "oneOf", v => ((INSGenerator)v).OneOf());
         manager.RegisterMessage(this, "noneOf", v => ((INSGenerator)v).NoneOf());
         manager.RegisterMessage(this, "more", v => ((INSGenerator)v).More);
      }

      public virtual void Reset()
      {
         index = -1;
         more = true;
      }

      public Value DoReset()
      {
         Reset();
         return null;
      }

      public virtual Value Next()
      {
         var next = generatorSource.Next(++index);
         more = !next.IsNil;
         return next;
      }

      public Value If() => If(this, Arguments);

      public Value IfNot() => IfNot(this, Arguments);

      public Value Map() => Map(this, Arguments);

      public Value MapIf() => MapIf(this, Arguments);

      public Value Skip() => Skip(this, Arguments);

      public Value SkipWhile() => SkipWhile(this, Arguments);

      public Value SkipUntil() => SkipUntil(this, Arguments);

      public Value Take() => Take(this, Arguments);

      public Value TakeWhile() => TakeWhile(this, Arguments);

      public Value TakeUntil() => TakeUntil(this, Arguments);

      public Value Group() => Group(this, Arguments);

      public Value Array() => GeneratorToArray(this);

      public Value FoldL() => FoldL(this, Arguments);

      public Value FoldR() => FoldR(this, Arguments);

      public Value AnyOf() => AnyOf(this, Arguments);

      public Value AllOf() => AllOf(this, Arguments);

      public Value OneOf() => OneOf(this, Arguments);

      public Value NoneOf() => NoneOf(this, Arguments);

      public Value Unique() => Unique(this, Arguments);

      public Value Flat() => Flat(this, Arguments);

      public Value First() => First(this, Arguments);

      public Region Region
      {
         get => region;
         set => region = value.Clone();
      }

      public override string ToString() => generatorSource.ToString();

      //public override Value AssignmentValue() => Array();

      public static Value Split(INSGenerator generator, Arguments arguments)
      {
         var iterator = new NSIterator(generator);
         iterator.Reset();
         using (var assistant = new ParameterAssistant(arguments))
         {
            var block = assistant.Block();
            var left = new Array();
            var right = new Array();
            if (block == null)
            {
               var count = arguments[0].Int;
               if (count == 0)
                  return generator.Array();

               Value value;
               for (var i = 0; i < count; i++)
               {
                  value = iterator.Next();
                  if (value.IsNil)
                     break;

                  left.Add(value);
               }

               value = iterator.Next();
               for (var i = 0; !value.IsNil && i < MAX_LOOP; i++)
               {
                  right.Add(value);
                  value = iterator.Next();
               }

               return new Array { left, right };
            }

            assistant.IteratorParameter();
            foreach (var item in iterator)
            {
               assistant.SetIteratorParameter(item);
               var result = block.IsTrue;
               var signal = Signal();
               if (signal == Breaking)
                  break;

               switch (signal)
               {
                  case Continuing:
                     continue;
                  case ReturningNull:
                     return null;
               }

               if (result)
                  left.Add(item);
               else
                  right.Add(item);
            }

            return new Array { left, right };
         }
      }

      public Value Split() => Split(this, Arguments);

      public Value SplitWhile() => SplitWhile(this, Arguments);

      public static Value SplitWhile(INSGenerator generator, Arguments arguments)
      {
         using (var assistant = new ParameterAssistant(arguments))
         {
            var block = assistant.Block();
            if (block == null)
               return generator.Array();

            var left = new Array();
            var right = new Array();
            var adding = true;
            assistant.IteratorParameter();
            var iterator = new NSIterator(generator);
            foreach (var item in iterator)
               if (adding)
               {
                  assistant.SetIteratorParameter(item);
                  var result = block.IsTrue;
                  var signal = Signal();
                  if (signal == Breaking)
                     break;

                  switch (signal)
                  {
                     case Continuing:
                        continue;
                     case ReturningNull:
                        return null;
                  }

                  if (result)
                     left.Add(item);
                  else
                  {
                     right.Add(item);
                     adding = false;
                  }
               }
               else
                  right.Add(item);

            return new Array { left, right };
         }
      }

      public Value SplitUntil() => SplitUntil(this, Arguments);

      public static Value SplitUntil(INSGenerator generator, Arguments arguments)
      {
         using (var assistant = new ParameterAssistant(arguments))
         {
            var block = assistant.Block();
            if (block == null)
               return generator.Array();

            var left = new Array();
            var right = new Array();
            var adding = true;
            assistant.IteratorParameter();
            var iterator = new NSIterator(generator);
            foreach (var item in iterator)
               if (adding)
               {
                  assistant.SetIteratorParameter(item);
                  var result = block.IsTrue;
                  var signal = Signal();
                  if (signal == Breaking)
                     break;

                  switch (signal)
                  {
                     case Continuing:
                        continue;
                     case ReturningNull:
                        return null;
                  }

                  if (!result)
                     left.Add(item);
                  else
                  {
                     right.Add(item);
                     adding = false;
                  }
               }
               else
                  right.Add(item);

            return new Array { left, right };
         }
      }

      public Value GatherWhile() => GatherWhile(this, Arguments);

      public Value GatherWhile(INSGenerator generator, Arguments arguments)
      {
         using (var assistant = new ParameterAssistant(arguments))
         {
            var block = assistant.Block();
            if (block == null)
               return this;

            assistant.IteratorParameter();

            var array = new Array();

            return array;
         }
      }

      public Region SharedRegion
      {
         get => sharedRegion;
         set
         {
            sharedRegion = value?.Clone();
            if (generatorSource is ISharedRegion sr)
               sr.SharedRegion = sharedRegion;
         }
      }
   }
}