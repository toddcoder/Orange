using System.Collections.Generic;
using Orange.Library.Managers;
using Standard.Types.Collections;
using Standard.Types.Enumerables;
using static Orange.Library.Runtime;
using static Orange.Library.Values.Ignore;
using static Orange.Library.Values.Nil;

namespace Orange.Library.Values
{
   public class NSGeneratorMod : Value, INSGenerator, ISharedRegion
   {
      public abstract class Modifier : INSGenerator, ISharedRegion
      {
         protected Parameters parameters;
         protected Block expression;
         protected Value value;
         protected int index;
         protected Region region;
         protected Region sharedRegion;

         protected Modifier(Arguments arguments)
         {
            parameters = arguments.Parameters;
            expression = arguments.Executable;
            index = -1;
            region = new Region();
         }

         public virtual void Reset() => index = -1;

         public Value DoReset() => null;

         public abstract Value Next();

         public Value If() => null;

         public Value IfNot() => null;

         public Value Map() => null;

         public Value MapIf() => null;

         public Value Skip() => null;

         public Value SkipWhile() => null;

         public Value SkipUntil() => null;

         public Value Take() => null;

         public Value TakeWhile() => null;

         public Value TakeUntil() => null;

         public Value Group() => null;

         public Value Array() => null;

         public Value FoldL() => null;

         public Value FoldR() => null;

         public Value AnyOf() => null;

         public Value AllOf() => null;

         public Value OneOf() => null;

         public Value NoneOf() => null;

         public Value Split() => null;

         public Value SplitWhile() => null;

         public Value SplitUntil() => null;

         public Value Unique() => null;

         public Value Flat() => null;

         public Value First() => null;

         public Value GatherWhile() => null;

         public Value GatherUntil() => null;

         public Region Region
         {
            get => region;
            set => region = value.Clone();
         }

         public INSGeneratorSource GeneratorSource => null;

         public void Visit(Value value) { }

         public bool More => false;

         public Value Evaluate(Value value)
         {
            if (value is Ignore)
               return value;

            this.value = value;
            using (var popper = new SharedRegionPopper(region, this, "modifier"))
            {
               popper.Push();
               parameters.SetValues(value, ++index);
               var next = Next();
               if (State.ReturnSignal)
                  State.ReturnSignal = false;
               return next;
            }
         }

         protected string argumentText() => $"(({parameters}) -> {expression})";

         public override string ToString() => argumentText();

         public Region SharedRegion
         {
            get => sharedRegion;
            set => sharedRegion = value?.Clone();
         }
      }

      public class IfModifier : Modifier
      {
         public IfModifier(Arguments arguments)
            : base(arguments) { }

         public override Value Next() => expression.Evaluate().IsTrue ? value : IgnoreValue;

         public override string ToString() => $"if {base.ToString()}";
      }

      public class IfNotModifier : Modifier
      {
         public IfNotModifier(Arguments arguments)
            : base(arguments) { }

         public override Value Next() => expression.Evaluate().IsTrue ? IgnoreValue : value;

         public override string ToString() => $"if not {base.ToString()}";
      }

      public class MapModifier : Modifier
      {
         public MapModifier(Arguments arguments)
            : base(arguments) { }

         public override Value Next()
         {
            var next = expression.Evaluate();
            if (next is None)
               return IgnoreValue;

            if (next is Some some)
               return some.Value();

            return next;
         }

         public override string ToString() => $"map {base.ToString()}";
      }

      public class MapIfModifier : Modifier
      {
         public MapIfModifier(Arguments arguments)
            : base(arguments) { }

         public override Value Next()
         {
            var result = expression.Evaluate();
            if (result.IsNull)
               return value;

            return result;
         }

         public override string ToString() => $"map if {base.ToString()}";
      }

      public class SkipModifier : Modifier
      {
         protected int count;

         public SkipModifier(Arguments arguments)
            : base(arguments) => count = arguments[0].Int;

         public override Value Next() => index < count ? IgnoreValue : value;

         public override string ToString() => $"skip {count}";
      }

      public class SkipWhileModifier : Modifier
      {
         protected bool applies;

         public SkipWhileModifier(Arguments arguments)
            : base(arguments) { }

         public override void Reset()
         {
            base.Reset();
            applies = false;
         }

         protected virtual bool condition() => expression.IsTrue;

         protected virtual Value ifTrue() => IgnoreValue;

         protected virtual Value ifFalse() => value;

         public override Value Next()
         {
            if (index == 0)
               applies = true;
            if (applies && condition())
               return ifTrue();

            applies = false;
            return ifFalse();
         }

         public override string ToString() => $"skip while {argumentText()}";
      }

      public class SkipUntilModifier : SkipWhileModifier
      {
         public SkipUntilModifier(Arguments arguments)
            : base(arguments) { }

         protected override bool condition() => !base.condition();

         public override string ToString() => $"skip until {argumentText()}";
      }

      public class TakeModifier : SkipModifier
      {
         public TakeModifier(Arguments arguments)
            : base(arguments) { }

         public override Value Next() => index < count ? value : NilValue;

         public override string ToString() => $"take {count}";
      }

      public class TakeWhileModifier : SkipWhileModifier
      {
         public TakeWhileModifier(Arguments arguments)
            : base(arguments) { }

         protected override Value ifTrue() => value;

         protected override Value ifFalse() => NilValue;

         public override string ToString() => $"take while {argumentText()}";
      }

      public class TakeUntilModifier : TakeWhileModifier
      {
         public TakeUntilModifier(Arguments arguments)
            : base(arguments) { }

         protected override bool condition() => !base.condition();

         public override string ToString() => $"take until {argumentText()}";
      }

      public class UniqueModifier : Modifier
      {
         Set<Value> set;

         public UniqueModifier(Arguments arguments)
            : base(arguments) => set = new Set<Value>();

         public override Value Next()
         {
            if (value.IsNil)
               return value;
            if (set.Contains(value))
               return IgnoreValue;

            set.Add(value);
            return value;
         }

         public override string ToString() => "unique";
      }

      INSGenerator generator;
      List<Modifier> modifiers;
      Region region;
      Region sharedRegion;
      NSIterator iterator;
      bool more;

      public NSGeneratorMod(INSGenerator generator, Modifier modifier)
      {
         this.generator = generator;
         modifiers = new List<Modifier> { modifier };
         region = new Region();
         iterator = new NSIterator(this.generator);
      }

      public NSGeneratorMod(NSGeneratorMod originalMod, Modifier modifier)
      {
         generator = originalMod.generator;
         modifiers = new List<Modifier>();
         modifiers.AddRange(originalMod.modifiers);
         modifiers.Add(modifier);
         region = originalMod.region;
         iterator = new NSIterator(generator);
      }

      public void Add(Modifier modifier) => modifiers.Add(modifier);

      public override int Compare(Value value) => 0;

      public override string Text
      {
         get { return GeneratorToArray(this).Text; }
         set { }
      }

      public override double Number { get; set; }

      public override ValueType Type => ValueType.Generator;

      public override bool IsTrue => false;

      public override Value Clone() => this;

      protected override void registerMessages(MessageManager manager)
      {
         manager.RegisterMessage(this, "reset", v => ((NSGeneratorMod)v).DoReset());
         manager.RegisterMessage(this, "next", v => ((NSGeneratorMod)v).Next());
         manager.RegisterMessage(this, "if", v => ((NSGeneratorMod)v).If());
         manager.RegisterMessage(this, "ifNot", v => ((NSGeneratorMod)v).IfNot());
         manager.RegisterMessage(this, "map", v => ((NSGeneratorMod)v).Map());
         manager.RegisterMessage(this, "mapIf", v => ((NSGeneratorMod)v).MapIf());
         manager.RegisterMessage(this, "skip", v => ((NSGeneratorMod)v).Skip());
         manager.RegisterMessage(this, "skipWhile", v => ((NSGeneratorMod)v).SkipWhile());
         manager.RegisterMessage(this, "skipUntil", v => ((NSGeneratorMod)v).SkipUntil());
         manager.RegisterMessage(this, "take", v => ((NSGeneratorMod)v).Take());
         manager.RegisterMessage(this, "takeWhile", v => ((NSGeneratorMod)v).TakeWhile());
         manager.RegisterMessage(this, "takeUntil", v => ((NSGeneratorMod)v).TakeUntil());
         manager.RegisterMessage(this, "array", v => ((NSGeneratorMod)v).Array());
         manager.RegisterMessage(this, "split", v => ((NSGeneratorMod)v).Split());
         manager.RegisterMessage(this, "splitWhile", v => ((NSGeneratorMod)v).SplitWhile());
         manager.RegisterMessage(this, "splitUntil", v => ((NSGeneratorMod)v).SplitUntil());
         manager.RegisterMessage(this, "unique", v => ((NSGeneratorMod)v).Unique());
         manager.RegisterMessage(this, "flat", v => ((NSGeneratorMod)v).Flat());
         manager.RegisterMessage(this, "first", v => ((NSGeneratorMod)v).First());
         manager.RegisterMessage(this, "group", v => ((NSGeneratorMod)v).Group());
         manager.RegisterMessage(this, "foldl", v => ((NSGeneratorMod)v).FoldL());
         manager.RegisterMessage(this, "foldr", v => ((NSGeneratorMod)v).FoldR());
         manager.RegisterMessage(this, "allOf", v => ((NSGeneratorMod)v).AllOf());
         manager.RegisterMessage(this, "anyOf", v => ((NSGeneratorMod)v).AnyOf());
         manager.RegisterMessage(this, "oneOf", v => ((NSGeneratorMod)v).OneOf());
         manager.RegisterMessage(this, "noneOf", v => ((NSGeneratorMod)v).NoneOf());
         manager.RegisterMessage(this, "more", v => ((NSGeneratorMod)v).More);
      }

      public Value DoReset()
      {
         Reset();
         return this;
      }

      public void Reset()
      {
         iterator.Reset();
         foreach (var modifier in modifiers)
            modifier.Reset();

         more = true;
      }

      public Value Next()
      {
         var value = iterator.Next();
         if (value.IsNil)
         {
            generator.Visit(value);
            more = false;
            return value;
         }

         foreach (var modifier in modifiers)
         {
            modifier.Region = Region;
            modifier.SharedRegion = sharedRegion;
            value = modifier.Evaluate(value);
            if (value.IsNil)
            {
               more = false;
               break;
            }
         }

         generator.Visit(value);
         return value;
      }

      public Value If() => new NSGeneratorMod(this, new IfModifier(Arguments) { SharedRegion = sharedRegion });

      public Value IfNot() => new NSGeneratorMod(this, new IfNotModifier(Arguments) { SharedRegion = sharedRegion });

      public Value Map() => new NSGeneratorMod(this, new MapModifier(Arguments) { SharedRegion = sharedRegion });

      public Value MapIf() => new NSGeneratorMod(this, new MapIfModifier(Arguments) { SharedRegion = sharedRegion });

      public Value Skip() => new NSGeneratorMod(this, new SkipModifier(Arguments) { SharedRegion = sharedRegion });

      public Value SkipWhile() => new NSGeneratorMod(this, new SkipWhileModifier(Arguments) { SharedRegion = sharedRegion });

      public Value SkipUntil() => new NSGeneratorMod(this, new SkipUntilModifier(Arguments) { SharedRegion = sharedRegion });

      public Value Take() => new NSGeneratorMod(this, new TakeModifier(Arguments) { SharedRegion = sharedRegion });

      public Value TakeWhile() => new NSGeneratorMod(this, new TakeWhileModifier(Arguments) { SharedRegion = sharedRegion });

      public Value TakeUntil() => new NSGeneratorMod(this, new TakeUntilModifier(Arguments) { SharedRegion = sharedRegion });

      public Value Group() => NSGenerator.Group(this, Arguments);

      public Value FoldL() => NSGenerator.FoldL(this, Arguments);

      public Value FoldR() => NSGenerator.FoldR(this, Arguments);

      public Value AnyOf() => NSGenerator.AnyOf(this, Arguments);

      public Value AllOf() => NSGenerator.AllOf(this, Arguments);

      public Value OneOf() => NSGenerator.OneOf(this, Arguments);

      public Value NoneOf() => NSGenerator.NoneOf(this, Arguments);

      public Value Unique() => NSGenerator.Unique(this, Arguments);

      public Value Flat() => NSGenerator.Flat(this, Arguments);

      public Value First() => NSGenerator.First(this, Arguments);

      public Region Region
      {
         get => region;
         set => region = value.Clone();
      }

      public INSGeneratorSource GeneratorSource => generator.GeneratorSource;

      public void Visit(Value value) { }

      public bool More => more;

      public override string ToString() => $"{generator} {modifiers.Listify(" ")}";

      public override Value AlternateValue(string message) => GeneratorToArray(this);

      public Value Array() => GeneratorToArray(this);

      public Value Split() => NSGenerator.Split(this, Arguments);

      public Value SplitWhile() => NSGenerator.SplitWhile(this, Arguments);

      public Value SplitUntil() => NSGenerator.SplitUntil(this, Arguments);

      public Region SharedRegion
      {
         get => sharedRegion;
         set => sharedRegion = value?.Clone();
      }
   }
}