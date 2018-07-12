using System.Collections.Generic;
using System.Linq;
using Orange.Library.Generators;
using Orange.Library.Managers;
using Standard.Types.Enumerables;
using Standard.Types.Strings;
using static Orange.Library.Managers.RegionManager;
using static Orange.Library.Runtime;
using static Orange.Library.Values.Generator.IterationControlType;
using static Orange.Library.Values.Generator.ValueUsageType;

namespace Orange.Library.Values
{
   public class Generator : Value, ISharedRegion
   {
      public enum IterationControlType
      {
         NA,
         Continuing,
         Skipping,
         Exiting
      }

      public enum ValueUsageType
      {
         NA,
         Conditional,
         Used,
         Ignored,
         Controlled
      }

      public abstract class Item
      {
         public Block Block { protected get; set; }

         public abstract Value Evaluate();

         public abstract IterationControlType Control { get; }

         public abstract ValueUsageType ValueUsage { get; }

         public int Count { get; set; }

         public abstract Item Clone();
      }

      class IfItem : Item
      {
         public override Value Evaluate() => Block.Evaluate();

         public override IterationControlType Control => IterationControlType.NA;

         public override ValueUsageType ValueUsage => Conditional;

         public override Item Clone() => new IfItem { Block = Block };

         public override string ToString() => $"if ({Block})";
      }

      class UnlessItem : Item
      {
         public override Value Evaluate() => !Block.Evaluate().IsTrue;

         public override IterationControlType Control => IterationControlType.NA;

         public override ValueUsageType ValueUsage => Conditional;

         public override Item Clone() => new UnlessItem { Block = Block };

         public override string ToString() => $"unless ({Block})";
      }

      class MapItem : Item
      {
         public override Value Evaluate() => Block.Evaluate();

         public override IterationControlType Control => Continuing;

         public override ValueUsageType ValueUsage => Used;

         public override Item Clone() => new MapItem { Block = Block };

         public override string ToString() => $"map ({Block})";
      }

      class TakeItem : Item
      {
         int taken;

         public TakeItem() => taken = 0;

         public override Value Evaluate() => ++taken <= Count;

         public override IterationControlType Control => Exiting;

         public override ValueUsageType ValueUsage => taken <= Count ? Conditional : Controlled;

         public override Item Clone() => new TakeItem { Block = Block, Count = Count };

         public override string ToString() => $"take {Count}";
      }

      class TakeWhileItem : Item
      {
         bool take;

         public TakeWhileItem() => take = true;

         public override Value Evaluate()
         {
            if (!Block.Evaluate().IsTrue)
               take = false;
            return take;
         }

         public override IterationControlType Control => Exiting;

         public override ValueUsageType ValueUsage => take ? Conditional : Controlled;

         public override Item Clone() => new TakeWhileItem { Block = Block };

         public override string ToString() => $"take while ({Block})";
      }

      public class TakeUntilItem : Item
      {
         bool take;

         public TakeUntilItem() => take = true;

         public override Value Evaluate()
         {
            if (Block.Evaluate().IsTrue)
               take = false;
            return take;
         }

         public override IterationControlType Control => Exiting;

         public override ValueUsageType ValueUsage => take ? Conditional : Controlled;

         public override Item Clone() => new TakeUntilItem { Block = Block };

         public override string ToString() => $"take until ({Block})";
      }

      class SkipItem : Item
      {
         int skipped;

         public SkipItem() => skipped = 0;

         public override Value Evaluate()
         {
            skipped++;
            return null;
         }

         public override IterationControlType Control => skipped <= Count ? Skipping : Continuing;

         public override ValueUsageType ValueUsage => Controlled;

         public override Item Clone() => new SkipItem { Block = Block, Count = Count };

         public override string ToString() => $"skip {Count}";
      }

      class SkipWhileItem : Item
      {
         bool skip;

         public SkipWhileItem() => skip = true;

         public override Value Evaluate()
         {
            if (!Block.Evaluate().IsTrue)
               skip = false;
            return skip;
         }

         public override IterationControlType Control => skip ? Skipping : Continuing;

         public override ValueUsageType ValueUsage => Controlled;

         public override Item Clone() => new SkipItem { Block = Block };

         public override string ToString() => $"skip while ({Block})";
      }

      class SkipUntilItem : Item
      {
         bool skip;

         public SkipUntilItem() => skip = true;

         public override Value Evaluate()
         {
            if (Block.Evaluate().IsTrue)
               skip = false;
            return skip;
         }

         public override IterationControlType Control => skip ? Skipping : Continuing;

         public override ValueUsageType ValueUsage => Controlled;

         public override Item Clone() => new SkipWhileItem { Block = Block };

         public override string ToString() => $"skip until ({Block})";
      }

      public static void SetParameter(string parameterName, Value value, string argumentName = null)
      {
         Regions.SetParameter(parameterName, value);
         Regions.SetParameter("$0", value);
         Regions.SetParameter(MangledName("0"), value);
         if (argumentName.IsNotEmpty())
            Regions.SetParameter(argumentName, value);
      }

      string parameterName;
      Value source;
      List<Item> items;
      int currentIndex;
      Value currentValue;
      IGenerator currentGenerator;
      Generator subGenerator;
      Region sharedRegion;

      public Generator(string parameterName, Value source)
      {
         this.parameterName = parameterName;
         this.source = source;
         if (source is Block block)
            block.Expression = false;
         items = new List<Item>();
         currentIndex = -1;
         currentValue = new Nil();
         currentGenerator = null;
         subGenerator = null;
      }

      public Generator(string parameterName, Value source, List<Item> items)
         : this(parameterName, source) => this.items.AddRange(items);

      public Generator(Value source)
         : this("$0", source) { }

      public string ParameterName => parameterName;

      public override int Compare(Value value) => 0;

      public override string Text
      {
         get => "";
         set { }
      }

      public override double Number { get; set; }

      public override ValueType Type => ValueType.Generator;

      public override bool IsTrue => false;

      public override Value Clone() => new Generator(parameterName, (Block)source.Clone(), items.Select(i => i.Clone()).ToList());

      protected override void registerMessages(MessageManager manager)
      {
         manager.RegisterMessage(this, "next", v => ((Generator)v).Next());
         manager.RegisterMessage(this, "reset", v => ((Generator)v).Reset());
         manager.RegisterMessage(this, "if", v => ((Generator)v).If());
         manager.RegisterMessage(this, "unless", v => ((Generator)v).Unless());
         manager.RegisterMessage(this, "map", v => ((Generator)v).Map());
         manager.RegisterMessage(this, "for", v => ((Generator)v).For());
         manager.RegisterMessage(this, "find", v => ((Generator)v).Find());
         manager.RegisterMessage(this, "scalar", v => ((Generator)v).Scalar());
         manager.RegisterMessage(this, "max", v => ((Generator)v).Max());
         manager.RegisterMessage(this, "min", v => ((Generator)v).Min());
         manager.RegisterMessage(this, "sum", v => ((Generator)v).Sum());
         manager.RegisterMessage(this, "avg", v => ((Generator)v).Avg());
         manager.RegisterMessage(this, "keep", v => ((Generator)v).Keep());
         manager.RegisterMessage(this, "group", v => ((Generator)v).Group());
         manager.RegisterMessage(this, "take", v => ((Generator)v).Take());
         manager.RegisterMessage(this, "takeWhile", v => ((Generator)v).TakeWhile());
         manager.RegisterMessage(this, "takeUntil", v => ((Generator)v).TakeUntil());
         manager.RegisterMessage(this, "skip", v => ((Generator)v).Skip());
         manager.RegisterMessage(this, "skipWhile", v => ((Generator)v).SkipWhile());
         manager.RegisterMessage(this, "skipUntil", v => ((Generator)v).SkipUntil());
         manager.RegisterMessage(this, "uniq", v => ((Generator)v).Unique());
         manager.RegisterMessage(this, "any?", v => ((Generator)v).Any());
         manager.RegisterMessage(this, "all?", v => ((Generator)v).All());
         manager.RegisterMessage(this, "none?", v => ((Generator)v).None());
         manager.RegisterMessage(this, "count", v => ((Generator)v).Count());
      }

      public Value Next()
      {
         if (currentGenerator == null)
            currentGenerator = GetGenerator();
         var looping = true;
         var region = new Region();
         using (var popper = new RegionPopper(region, "generator-next"))
         {
            popper.Push();
            sharedRegion?.CopyAllVariablesTo(region);
            for (var i = currentIndex + 1; i < MAX_ARRAY && looping; i++)
            {
               var value = getNext(i, out var control);
               if (value.IsNil)
                  return value;

               switch (control)
               {
                  case Continuing:
                     currentIndex = i;
                     currentValue = value;
                     return value;
                  case Skipping:
                     currentIndex = i;
                     currentValue = value;
                     continue;
                  case Exiting:
                     looping = false;
                     break;
               }
            }

            currentIndex = -1;
            currentValue = new Nil();
            return currentValue;
         }
      }

      Value getNext(int i, out IterationControlType control)
      {
         Value value;
         if (subGenerator != null)
         {
            value = subGenerator.Next();
            if (value.IsNil)
               subGenerator = null;
            else
            {
               control = Continuing;
               return value;
            }
         }

         value = GetNext(currentGenerator, i, out control);
         if (value is Generator aSubGenerator)
         {
            subGenerator = aSubGenerator;
            value = subGenerator.Next();
         }
         return value;
      }

      public Value Reset()
      {
         currentGenerator = null;
         currentIndex = -1;
         currentValue = new Nil();
         return this;
      }

      Generator addItem<TItem>(int count = 0, Arguments arguments = null)
         where TItem : Item, new()
      {
         var item = new TItem { Block = arguments == null ? Arguments.Executable : arguments.Executable, Count = count };
         var newGenerator = (Generator)Clone();
         newGenerator.items.Add(item);
         return newGenerator;
      }

      public Value If() => addItem<IfItem>();

      public Value Unless() => addItem<UnlessItem>();

      public Value Map() => addItem<MapItem>();

      public Value For()
      {
         var framework = new ForFramework(this, Arguments.Executable, Arguments);
         return framework.Evaluate();
      }

      public Value Find()
      {
         var framework = new FindFramework(this, Arguments.Executable, Arguments);
         return framework.Evaluate();
      }

      public IGenerator GetGenerator()
      {
         var region = new Region();
         sharedRegion?.CopyAllVariablesTo(region);
         using (var popper = new RegionPopper(region, "get-generator"))
         {
            popper.Push();
            var value = source is IExecutable e ? e.Evaluate() : source;
            if (value is IGenerator generator)
               return generator;

            if (value is IGetGenerator getGenerator)
               return getGenerator.GetGenerator();

            return new DefaultGenerator(value);
         }
      }

      public Value GetNext(IGenerator generator, int index, out IterationControlType control)
      {
         SetParameter(parameterName, currentValue);
         var value = generator.Next(index);
         if (value.IsNil)
         {
            control = Exiting;
            return value;
         }

         var looping = true;
         control = Continuing;
         foreach (var item in items.TakeWhile(item => looping))
         {
            SetParameter(parameterName, value);
            var result = item.Evaluate();
            switch (item.ValueUsage)
            {
               case Conditional:
                  if (result.IsTrue)
                     control = Continuing;
                  else
                  {
                     control = Skipping;
                     looping = false;
                  }
                  break;
               case Used:
                  value = result;
                  break;
               case Ignored:
                  break;
               case Controlled:
                  control = item.Control;
                  switch (control)
                  {
                     case Skipping:
                     case Exiting:
                        looping = false;
                        break;
                  }

                  break;
            }
         }

         return value;
      }

      Array getArray()
      {
         var framework = new ArrayFramework(this, Arguments);
         return (Array)framework.Evaluate();
      }

      public Value Scalar()
      {
         var framework = new ScalarFramework(this, Arguments.Executable, Arguments);
         return framework.Evaluate();
      }

      public Value Max()
      {
         var framework = new MaxFramework(this, Arguments.Executable, Arguments);
         return framework.Evaluate();
      }

      public Value Min()
      {
         var framework = new MinFramework(this, Arguments.Executable, Arguments);
         return framework.Evaluate();
      }

      public Value Sum()
      {
         var framework = new SumFramework(this, Arguments.Executable, Arguments);
         return framework.Evaluate();
      }

      public Value Avg()
      {
         var framework = new AverageFramework(this, Arguments.Executable, Arguments);
         return framework.Evaluate();
      }

      public Value Keep()
      {
         var framework = new KeepFramework(this, Arguments.Executable, Arguments);
         return framework.Evaluate();
      }

      public Value Group()
      {
         var framework = new GroupFramework(this, Arguments.Executable, Arguments);
         return framework.Evaluate();
      }

      public Value Take() => addItem<TakeItem>((int)Arguments[0].Number);

      public Value TakeWhile() => addItem<TakeWhileItem>(arguments: Arguments);

      public Value TakeUntil() => addItem<TakeUntilItem>(arguments: Arguments);

      public Value Skip() => addItem<SkipItem>((int)Arguments[0].Number);

      public Value SkipWhile() => addItem<SkipWhileItem>(arguments: Arguments);

      public Value SkipUntil() => addItem<SkipUntilItem>(arguments: Arguments);

      public Value Unique()
      {
         var framework = new UniqueFramework(this, Arguments);
         return framework.Evaluate();
      }

      public Value Any()
      {
         var framework = new AnyFramework(this, Arguments.Executable, Arguments);
         return framework.Evaluate();
      }

      public Value All()
      {
         var framework = new AllFramework(this, Arguments.Executable, Arguments);
         return framework.Evaluate();
      }

      public Value None()
      {
         var framework = new NoneFramework(this, Arguments.Executable, Arguments);
         return framework.Evaluate();
      }

      public Value Count()
      {
         var framework = new CountFramework(this, Arguments);
         return framework.Evaluate();
      }

      public override bool IsArray => true;

      public override Value SourceArray => getArray();

      public override Value AlternateValue(string message) => getArray();

      public override string ToString()
      {
         var builder = new System.Text.StringBuilder();
         builder.Append($"({parameterName} <- {source})");
         if (items.Count > 0)
            builder.Append($" {items.Listify(" ")}");
         return builder.ToString();
      }

      public bool Match(Array comparisand, bool required, bool assigning)
      {
         var length = comparisand.Length;
         if (comparisand.Values.All(v => v.Type == ValueType.Placeholder) && length > 0)
            return bindTo(comparisand, assigning);

         foreach (var item in comparisand)
         {
            var next = Next();
            if (next.IsNil)
               return false;

            switch (item.Value.Type)
            {
               case ValueType.Placeholder:
                  Regions.SetBinding(item.Value.Text, next, assigning);
                  break;
               case ValueType.Any:
                  continue;
            }

            if (!Case.Match(next, item.Value, required, null))
               return false;
         }

         return true;
      }

      bool bindTo(Array placeholderArray, bool assigning)
      {
         var length = placeholderArray.Length;
         if (length == 0)
            return false;

         var last = length - 1;
         for (var i = 0; i < last; i++)
         {
            var next = Next();
            if (next.IsNil)
               return false;

            Regions.SetBinding(placeholderArray[i].Text, next, assigning);
         }

         Regions.SetBinding(placeholderArray[last].Text, this, assigning);
         return true;
      }

      public Region SharedRegion
      {
         get => sharedRegion;
         set => sharedRegion = value;
      }
   }
}