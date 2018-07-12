using System;
using System.Collections.Generic;
using System.Linq;
using Orange.Library.Managers;
using Standard.Types.Enumerables;
using Standard.Types.Maybe;
using static Orange.Library.Runtime;
using static Orange.Library.Values.Case;
using static Orange.Library.Values.Nil;
using static Orange.Library.Values.Parameters;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Orange.Library.Values
{
   public class MultiLambda : Value, IXMethod, IHasRegion, IInvokeable, IWhere
   {
      const string LOCATION = "MultiLambda";

      protected string functionName;
      protected List<MultiLambdaItem> items;
      protected bool memoize;
      protected Lazy<Memoizer> memo;

      public MultiLambda(string functionName, bool memoize)
      {
         this.functionName = functionName;
         items = new List<MultiLambdaItem>();
         this.memoize = memoize;
         memo = new Lazy<Memoizer>(() => new Memoizer());
      }

      public virtual void Add(MultiLambdaItem item)
      {
         items.Add(item);
         if (item.Where != null)
            Where = item.Where;
      }

      public override int Compare(Value value) => 0;

      public override string Text { get; set; }

      public override double Number { get; set; }

      public override ValueType Type => ValueType.MultiLambda;

      public override bool IsTrue => false;

      public override Value Clone() => new MultiLambda(functionName, memoize)
      {
         items = items.Select(i => (MultiLambdaItem)i.Clone()).ToList()
      };

      protected override void registerMessages(MessageManager manager)
      {
         manager.RegisterMessage(this, "invoke", v => ((MultiLambda)v).Invoke());
         manager.RegisterMessage(this, "exp", v => ((MultiLambda)v).ExpandedBlock());
         manager.RegisterMessage(this, "apply", v => ((MultiLambda)v).Apply());
      }

      public Value Apply()
      {
         var arguments = Arguments.FromValue(Arguments.ApplyValue, false);
         return Invoke(arguments);
      }

      public Value Invoke() => items.All(i => i.Expand) ? InvokeExpanded(Arguments) : Invoke(Arguments);

      public Value InvokeExpanded(Arguments arguments)
      {
         var block = getExpandedBlock(arguments);
         return block == null ? new Nil() : block.Evaluate();
      }

      Block getExpandedBlock(Arguments arguments)
      {
         var region = new Region();
         using (var popper = new RegionPopper(region, "invoke expanded"))
         {
            Assert(items.Count == 2, LOCATION, "There must be exactly two functions");
            var item0 = items[0];
            var item1 = items[1];
            var lambda0 = item0.Lambda;
            var lambda1 = item1.Lambda;
            var parameters0 = lambda0.Parameters;
            var parameters1 = lambda1.Parameters;
            Assert(parameters0.AnyComparisands, LOCATION, "The first function must have a comparisand");

            var values = parameters1.GetArguments(arguments);
            popper.Push();

            (var checkExpression, var checkVariable) = firstComparisand(lambda0.Parameters);
            var expander = new Expander(functionName, item1.Lambda.Block, checkExpression, checkVariable, lambda0.Block, region);

            SetArguments(values);
            foreach (var parameter in parameters1.GetParameters())
               expander.AddParameter(parameter);

            var block = expander.Expand();
            return block;
         }
      }

      static (Block, string) firstComparisand(Parameters parameters)
      {
         var selectedParameter = parameters.GetParameters().FirstOrDefault(p => p.Comparisand != null);
         RejectNull(selectedParameter, LOCATION, $"No comparisands in parameters {parameters}");
         return (selectedParameter.Comparisand, selectedParameter.Name);
      }

      public override string ToString() => items.Select(i => i.ToString()).Listify();

      protected static bool canInvoke(Parameters parameters, List<ParameterValue> values, bool required)
      {
         switch (parameters.Length)
         {
            case 0:
               return true;
            case 1 when parameters[0].Comparisand[0].PushType.FlatMap(pt => pt, () => ValueType.Nil) == ValueType.Any:
               return true;
         }

         if (parameters.Length != values.Count)
            return false;

         for (var i = 0; i < parameters.Length; i++)
         {
            var parameter = parameters[i];
            var value = values[i].Value;
            if (canInvoke(parameter, value, required))
               continue;

            return false;
         }

         return true;
      }

      protected static bool canInvoke(Parameter parameter, Value value, bool required)
      {
         var comparisand = parameter.Comparisand;
         var right = comparisand?.Evaluate();
         if (right == null)
            return true;

         return Match(value, right, required, null) && (parameter.Condition?.IsTrue ?? true);
      }

      public virtual Value Invoke(Arguments arguments)
      {
         if (items.All(i => i.Expand))
            return InvokeExpanded(arguments);

         arguments.DefaultValue = new Nil();
         foreach (var item in items)
         {
            var region = new Region();
            using (var popper = new RegionPopper(region, "multi-lambda"))
            {
               var parameters = item.Lambda.Parameters;
               var values = parameters.GetArguments(arguments);
               if (memoize)
                  memo.Value.Evaluate(values);
               popper.Push();
               SetArguments(values);
               if (canInvoke(parameters, values, item.Required))
               {
                  ExecuteWhere(this);
                  if (!(parameters.Condition?.Evaluate().IsTrue ?? true))
                     continue;
                  if (!(item.Condition?.Evaluate().IsTrue ?? true))
                     continue;

                  if (Region != null)
                     item.Lambda.Region = Region;
                  var item1 = item;
                  var result = memoize ? memo.Value.Evaluate(() => evaluate(arguments, item1)) : evaluate(arguments, item);
                  /*                  if (result != null && result.IsNil)
                                       continue;*/
                  return result ?? NilValue;
               }
            }
         }

         Throw(LOCATION, $"No match found for ({arguments})");
         return NilValue;
      }

      static Value evaluate(Arguments arguments, MultiLambdaItem item) =>
         item.Lambda.Evaluate(arguments, register: true, setArguments: false);

      public Value ExpandedBlock() => getExpandedBlock(Arguments);

      public List<MultiLambdaItem> Items => items;

      public bool XMethod
      {
         get => items.All(i => i.Lambda.XMethod);
         set { }
      }

      public Region Region { get; set; }

      public bool ImmediatelyInvokeable { get; set; }

      public int ParameterCount => items.Select(i => i.Lambda.ParameterCount).Max();

      public bool Matches(Signature signature) => items.Any(i => i.Lambda.ParameterCount == signature.ParameterCount);

      public bool Initializer { get; set; }

      public IMaybe<ObjectRegion> ObjectRegion { get; set; } = none<ObjectRegion>();

      public Block Where { get; set; }
   }
}