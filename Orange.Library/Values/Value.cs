using System;
using System.Collections.Generic;
using System.Numerics;
using Orange.Library.Managers;
using Orange.Library.Verbs;
using Standard.Types.Maybe;
using Standard.Types.Numbers;
using Standard.Types.Strings;
using static Orange.Library.CodeBuilder;
using static Orange.Library.Compiler;
using static Orange.Library.Managers.MessageManager;
using static Orange.Library.Managers.RegionManager;
using static Orange.Library.Runtime;
using static Orange.Library.Values.Parameters;
using static Standard.Types.Maybe.MaybeFunctions;
using CSComplex = System.Numerics.Complex;

namespace Orange.Library.Values
{
   public abstract class Value
   {
      public enum ValueType
      {
         String,
         Number,
         Boolean,
         Block,
         Pattern,
         Array,
         KeyedValue,
         Lambda,
         Partition,
         Variable,
         Date,
         Indexer,
         StringIndexer,
         System,
         Nil,
         Ternary,
         Format,
         PatternResult,
         Parameters,
         StopIncrement,
         Proxy,
         Message,
         Between,
         XML,
         Template,
         Buffer,
         Padder,
         Cube,
         Range,
         FArray,
         FLines,
         StringBuffer,
         FunctionCall,
         Records,
         Iterator,
         Lines,
         RangeRepeater,
         Table,
         XMLElement,
         Case,
         Symbol,
         Enumeration,
         Module,
         ModuleVariable,
         Match,
         Invoker,
         DotNetObject,
         ArrayBuilder,
         ArrayGenerator,
         Binding,
         Assembly,
         Type,
         Member,
         Word,
         ReplacementLiteral,
         FieldVariable,
         When,
         Comprehension,
         MapIf,
         ArrayStream,
         Sequence,
         ArrayYielder,
         ObjectSequence,
         CFor,
         EnumerationItem,
         ArrayWrapper,
         Error,
         PrintBlock,
         InternalList,
         ListField,
         Separator,
         ObjectDynamicVariable,
         Global,
         NamespaceVariable,
         MessageInvoke,
         Interval,
         DateRange,
         Thunk,
         Any,
         Rational,
         MessagePath,
         Placeholder,
         Alternation,
         Complex,
         Big,
         CaseAnd,
         AlternationStream,
         MultiLambdaItem,
         MultiLambda,
         TypeName,
         Concatenation,
         File,
         FileReader,
         FileWriter,
         StringNavigator,
         ParallelZip,
         Graph,
         GraphVariable,
         Recurser,
         FunctionReference,
         Class,
         InvokeableReference,
         ObjectVariable,
         Abstract,
         ToDo,
         ObjectIndexer,
         If,
         Signature,
         Trait,
         Delegate,
         Set,
         Pending,
         MessageArguments,
         Null,
         ValueAttributeVariable,
         Object,
         AutoInvoker,
         PseudoRecursion,
         Macro,
         BlockPattern,
         Grammar,
         Pattern2,
         Unto,
         MessageData,
         Some,
         None,
         LazyBlock,
         Generator,
         GeneratorSource,
         Pair,
         ContractInvokeable,
         BoundValue,
         VerbBinding,
         Folder,
         YieldGenerator,
         FunctionApplication,
         Regex,
         RegexResult,
         ChooseIndexer,
         Consts,
         List,
         FunctionChain,
         Scanner,
         ListIndexer,
         Ignore,
         Tuple,
         KeyValue,
         Failure,
         Generate,
         Record,
         MessageSet,
         Constructor,
         Data
      }

      [Flags]
      public enum OptionType
      {
         None = 1,
         LJust = 2,
         RJust = 4,
         Center = 8,
         Max = 16,
         NoPad = 32,
         Case = 64,
         Anchor = 128,
         Numeric = 256,
         Descending = 512,
         Flat = 1028
      }

      public static implicit operator Value(double value) => new Double(value);

      public static implicit operator Value(string value) => new String(value);

      public static implicit operator Value(bool value) => new Boolean(value);

      public static implicit operator Value(DateTime value) => new Date(value);

      public static implicit operator Value(CSComplex value) => new Complex(value);

      public static implicit operator Value(BigInteger value) => new Big(value);

      protected readonly long id;
      protected Bits32<OptionType> options;

      public Value()
      {
         id = CompilerState.ObjectID();
         PerformElse = null;
         options = OptionType.None;
      }

      public override bool Equals(object obj) => obj is Value value && (id == value.id || Compare(value) == 0);

      public override int GetHashCode() => id.GetHashCode();

      public static bool operator ==(Value x, Value y)
      {
         if (ReferenceEquals(null, x))
            return ReferenceEquals(null, y);
         if (ReferenceEquals(null, y))
            return false;
         if (x.id == y.id)
            return true;

         return x.Compare(y) == 0;
      }

      public static bool operator !=(Value x, Value y) => !(x == y);

      public abstract int Compare(Value value);

      public abstract string Text { get; set; }

      public abstract double Number { get; set; }

      public abstract ValueType Type { get; }

      public abstract bool IsTrue { get; }

      public virtual Value AssignmentValue() => this;

      public virtual Value ArgumentValue() => this;

      public virtual void AssignTo(Variable variable) => variable.Value = AssignmentValue();

      public virtual Value Resolve() => this;

      public abstract Value Clone();

      public bool IsNumeric() => Text.IsNumeric();

      public long ID => id;

      public virtual Value Do(bool repeat) => this;

      public virtual Value Do(int count) => this;

      public virtual bool IsVariable => false;

      protected abstract void registerMessages(MessageManager manager);

      public void RegisterMessages()
      {
         registerMessages(MessagingState);
      }

      public void RegisterUserMessage(Message message)
      {
         MessagingState.RegisterMessage(this, message.MessageName,
            v => EvaluateMessage(v, message.MessageArguments.Parameters, message.MessageArguments.Executable));
      }

      public static Value EvaluateMessage(Value value, Parameters parameters, Block block)
      {
         List<ParameterValue> values = null;
         if (!(parameters is NullParameters))
            values = parameters.GetArguments(value.Arguments);
         block.AutoRegister = false;
         State.RegisterBlock(block);
         Regions.SetLocal("self", value);
         if (values != null)
            SetArguments(values);
         var result = block.Evaluate();
         result = State.UseReturnValue(result);
         State.UnregisterBlock();
         return result;
      }

      public Arguments Arguments { get; set; }

      public virtual Value AlternateValue(string message) => null;

      public virtual bool IsEmpty => Type == ValueType.String && Text.IsEmpty() || Type == ValueType.Nil ||
         Type == ValueType.Null;

      public virtual string ContainerType => Type.ToString();

      public bool? PerformElse { get; set; }

      public Value SetPerformElse(bool value)
      {
         PerformElse = value;
         return this;
      }

      public Message Message1 { get; set; }

      public virtual bool IsArray => false;

      public virtual Value SourceArray => null;

      public virtual Value SliceArray => null;

      public Array YieldValue { get; set; }

      public void SetOption(string option)
      {
         var value = option.ToEnumeration<OptionType>();
         options[value] = true;
      }

      public void SetOption(OptionType option) => options[option] = true;

      public void SetOptions(string[] options)
      {
         foreach (var option in options)
            SetOption(option);
      }

      public Bits32<OptionType> Options
      {
         get => options;
         set => options = value;
      }

      public virtual bool IsIndexer => false;

      public virtual bool IsExecutable => false;

      public virtual Block AsBlock => null;

      public virtual bool IsNil => false;

      public virtual bool IsNull => false;

      public virtual bool IsIgnore => false;

      public Block Pushed => PushValue(this);

      public Verb PushedVerb => new Push(this);

      public virtual Value Self => this;

      public int Int => (int)Number;

      public virtual IMaybe<INSGenerator> PossibleGenerator()
      {
         switch (this)
         {
            case INSGenerator generator:
               return generator.Some();
            case INSGeneratorSource source when source.IsGeneratorAvailable:
               return source.GetGenerator().Some();
            default:
               return none<INSGenerator>();
         }
      }

      public virtual IMaybe<INSGenerator> PossibleIndexGenerator() => PossibleGenerator();

      public virtual bool ProvidesGenerator => this is INSGenerator || this is INSGeneratorSource;
   }
}