using System.Collections.Generic;
using System.Linq;
using Orange.Library.Managers;
using Standard.Types.Maybe;
using static Orange.Library.Managers.RegionManager;
using static Orange.Library.Runtime;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Orange.Library.Values
{
   public class Lambda : Value, IExecutable, IXMethod, IHasRegion, IInvokeable, IImmediatelyInvokeable, IMacroBlock,
      IWhere, ISharedRegion, INSGeneratorSource
   {
      public class LambdaGenerator : NSGenerator
      {
         protected INSGenerator block;
         protected IMaybe<ObjectRegion> objectRegion;

         public LambdaGenerator(Lambda lambda)
            : base(lambda)
         {
            region = lambda.region;
            sharedRegion = lambda.sharedRegion;
            block = lambda.block.GetGenerator();
            objectRegion = lambda.ObjectRegion;
         }

         public override void Reset()
         {
            base.Reset();

            block.Reset();
            region?.RemoveAll();
         }

         public override Value Next()
         {
            using (var outerPopper = new ObjectRegionPopper(objectRegion, "object-region"))
            {
               outerPopper.Push();

               using (var innerPopper = new SharedRegionPopper(region, sharedRegion, "lambda-generator"))
               {
                  innerPopper.Push();

                  var next = block.Next();
                  more = !next.IsNil;
                  return next;
               }
            }
         }
      }

      const string LOCATION = "Lambda";

      public static Lambda FromBlock(Block block) => new Lambda(new Region(), block, new Parameters(), false);

      protected Region region;
      protected Region sharedRegion;
      protected Block block;
      protected Parameters parameters;
      protected bool enclosing;

      public Lambda(Region region, Block block, Parameters parameters, bool enclosing)
      {
         this.region = region;
         this.block = block;
         this.block?.RemoveEndingEnds();
         this.parameters = parameters;
         this.enclosing = enclosing;
      }

      public Lambda()
      {
         region = new Region();
         block = new Block();
         parameters = new Parameters();
         enclosing = false;
      }

      public Parameters Parameters => parameters;

      public Block Block => block;

      public string TailCallName { get; set; }

      protected IMaybe<Lambda> preamble(Arguments arguments, bool setArguments, bool register, Value instance)
      {
         block.AutoRegister = false;

         if (setArguments)
         {
            List<ParameterValue> values;
            bool partial;
            evaluateArguments(arguments, out values, out partial, register);
            if (partial)
               return createPartial(values).Some();
         }

         Regions.SetParameter("this", this);
         Regions.Current.Instance = instance;
         ExecuteWhere(this);

         State.RegisterLambdaRegion(Regions.Current);

         return none<Lambda>();
      }

      protected void registerBlock(bool register)
      {
         if (register)
         {
            if (enclosing)
               State.RegisterBlock(block, new Region());
            else
               State.RegisterBlock(block);
            if (sharedRegion != null)
               Regions.Push(sharedRegion, "shared");
         }
      }

      protected Value postscript(Value result, bool register, bool setArguments)
      {
         if (result is ISharedRegion sr)
            sr.SharedRegion = Regions.Current;
         if (register && setArguments)
         {
            if (sharedRegion != null && Regions.Current.Name == "shared")
               Regions.Pop("shared");
            State.UnregisterBlock();
         }
         State.UnregisterLambdaRegion();
         return result;
      }

      public virtual Value Evaluate(Arguments arguments, Value instance = null, bool register = true,
         bool setArguments = true)
      {
         var partialLambda = preamble(arguments, setArguments, register, instance);
         if (partialLambda.IsSome)
            return partialLambda.Value;

         var result = evaluateBlock();

         return postscript(result, register, setArguments);
      }

      protected virtual Lambda createPartial(List<ParameterValue> values)
      {
         var builder = new CodeBuilder();
         foreach (var parameterValue in values)
         {
            var parameterName = parameterValue.Name;
            if (parameterValue.Value.Type == ValueType.Any)
            {
               builder.Parameter(parameterName);
               builder.VariableAsArgument(parameterName);
            }
            else
            {
               builder.Define(parameterName);
               builder.Assign();
               builder.Value(parameterValue.Value);
               builder.End();
            }
         }

         builder.Inline(block);
         return builder.Lambda();
      }

      protected virtual Value evaluateBlock()
      {
         if (block.Yielding)
            return new LambdaGenerator(this);

         var result = block.Evaluate();
         result = State.UseReturnValue(result);
         return result;
      }

      protected virtual void evaluateArguments(Arguments arguments, out List<ParameterValue> values, out bool partial,
         bool register)
      {
         values = parameters.GetArguments(arguments);
         if (values.Any(v => v.Value.Type == ValueType.Any))
         {
            partial = true;
            return;
         }

         partial = false;
         registerBlock(register);
         Parameters.SetArguments(values);
      }

      public Value EvaluateNoArguments()
      {
         block.AutoRegister = false;
         State.RegisterBlock(block, block.ResolveVariables);
         var result = block.Evaluate();
         result = State.UseReturnValue(result);
         State.UnregisterBlock();
         return result;
      }

      public bool Enclosing => enclosing;

      public override int Compare(Value value) => value is Lambda l ? parameters.Compare(l.parameters) + block.Compare(l.block) : -2;

      public override string Text
      {
         get => "";
         set { }
      }

      public override double Number
      {
         get => 0;
         set { }
      }

      public override ValueType Type => ValueType.Lambda;

      public override bool IsTrue => false;

      public override Value Clone() => new Lambda(region, (Block)block.Clone(), parameters, enclosing);

      public Value Evaluate(bool invoked = false) => Evaluate(Arguments);

      protected override void registerMessages(MessageManager manager)
      {
         manager.RegisterMessage(this, "invoke", v => ((Lambda)v).Evaluate(true));
         manager.RegisterMessageCall("apply");
         manager.RegisterMessage(this, "apply", v => ((Lambda)v).Apply());
         manager.RegisterMessageCall("applyWhile");
         manager.RegisterMessage(this, "applyWhile", v => ((Lambda)v).Apply());
         manager.RegisterMessage(this, "repeat", v => ((Lambda)v).Repeat());
         manager.RegisterMessage(this, "params", v => ((Lambda)v).Params());
         manager.RegisterMessage(this, "block", v => ((Lambda)v).GetBlock());
         manager.RegisterMessage(this, "tabulate", v => ((Lambda)v).Tabulate());
         manager.RegisterMessage(this, "shr", v => ((Lambda)v).ShiftRight());
      }

      public override string ToString() => $"(({parameters}) {(Splatting ? "=>" : "->")} {block})";

      public virtual Value Invoke(Arguments arguments) => Evaluate(arguments);

      public Value Evaluate() => EvaluateNoArguments();

      public Block Action => block;

      public Lambda AsLambda => this;

      public Value Apply()
      {
         switch (Arguments.ApplyValue)
         {
            case Block yieldBlock:
               return new Comprehension(yieldBlock, parameters) { ArrayBlock = block };
            case Comprehension comprehension:
               return comprehension.PushDownComprehension(this);
            case var applyValue:
               return Evaluate(new Arguments(applyValue));
         }
      }

      public Value GetBlock() => block;

      public void ShareNamespace() => Regions.Current.CopyAllVariablesTo(region);

      public Region Region
      {
         get => region;
         set
         {
            RejectNull(value, LOCATION, "Internal error: region is null");
            region = new LambdaRegion(value);
         }
      }

      public override bool IsExecutable => true;

      public Value Repeat()
      {
         var count = (int)Arguments[0].Number;
         var array = new Array();
         using (var assistant = new ParameterAssistant(ParameterBlock.FromExecutable(this)))
         {
            assistant.IteratorParameter();
            for (var i = 0; i < count; i++)
            {
               assistant.SetIteratorParameter(i);
               var value = block.Evaluate();
               array.Add(value);
            }

            return array;
         }
      }

      public bool Splatting { get; set; }

      public Value Params() => parameters;

      public bool XMethod { get; set; }

      public string Message { get; set; }

      public override void AssignTo(Variable variable)
      {
         if (XMethod)
         {
            Message = variable.Name;
            MarkAsXMethod(Message, this);
         }
         base.AssignTo(variable);
      }

      public bool ImmediatelyInvokeable { get; set; }

      public int ParameterCount => parameters.Length;

      public bool Matches(Signature signature) => parameters.Length == signature.ParameterCount;

      public bool Initializer { get; set; }

      public IMaybe<ObjectRegion> ObjectRegion { get; set; } = none<ObjectRegion>();

      public bool Expand { get; set; }

      public Block MacroBlock => block;

      public virtual Block Where { get; set; }

      public int YieldCount => block.YieldCount;

      public Region SharedRegion
      {
         get => sharedRegion;
         set => sharedRegion = value?.Clone();
      }

      public Value Tabulate()
      {
         var length1 = Arguments[0].Int;
         var length2 = Arguments[1].Int;
         return length2 > 0 ? tabulateDimensions(length1, length2) : tabulateDimension(length1);
      }

      Array tabulateDimension(int length) => new Array(Enumerable.Range(0, length).Select(i => Evaluate(new Arguments(i))));

      Array tabulateDimensions(int length1, int length2)
      {
         var array = new Array();
         for (var i = 0; i < length1; i++)
         {
            var innerArray = new Array();
            for (var j = 0; j < length2; j++)
            {
               var arguments = new Arguments(new Value[] { i, j });
               innerArray.Add(Evaluate(arguments));
            }

            array.Add(innerArray);
         }

         return array;
      }

      public Value ShiftRight()
      {
         if (Arguments[0] is Lambda rightLambda)
         {
            var chain = new FunctionChain();
            chain.Add(this);
            chain.Add(rightLambda);
            return chain;
         }
         Throw(LOCATION, "Right hand value must be a lambda");
         return null;
      }

      public INSGenerator GetGenerator() => new LambdaGenerator(this);

      public Value Next(int index) => null;

      public bool IsGeneratorAvailable => true;

      public Array ToArray() => Runtime.ToArray(GetGenerator());
   }
}