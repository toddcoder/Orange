using Orange.Library.Managers;
using Standard.Types.Maybe;
using static Orange.Library.Runtime;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Orange.Library.Values
{
   public class ContractInvokeable : Value, IInvokeable
   {
      const string LOCATION = "Contract Invokeable";

      public IInvokeable Main { get; set; }

      public IInvokeable Require { get; set; }

      public IInvokeable Ensure { get; set; }

      public string Name { get; set; }

      public override int Compare(Value value) => 0;

      public override string Text { get; set; }

      public override double Number { get; set; }

      public override ValueType Type => ValueType.ContractInvokeable;

      public override bool IsTrue => false;

      static IInvokeable cloneInvokeable(IInvokeable value) => (IInvokeable)((Value)value)?.Clone();

      public override Value Clone() => new ContractInvokeable
      {
         Main = cloneInvokeable(Main),
         Require = cloneInvokeable(Require),
         Ensure = cloneInvokeable(Ensure),
         Name = Name
      };

      protected override void registerMessages(MessageManager manager) { }

      public Value Invoke(Arguments arguments)
      {
         Value result;
         if (Require != null)
         {
            result = Require.Invoke(arguments);
            Assert(result.IsTrue, LOCATION, $"Require for {Name} failed");
         }
         RejectNull(Main, LOCATION, $"Main invokeable for {Name} not created");
         result = Main.Invoke(arguments);
         if (Ensure != null)
         {
            var ensureArguments = new Arguments(result);
            var ensureResult = Ensure.Invoke(ensureArguments);
            Assert(ensureResult.IsTrue, LOCATION, $"Ensure for {Name} failed");
         }
         return result;
      }

      public Region Region { get; set; }

      public bool ImmediatelyInvokeable { get; set; }

      public int ParameterCount => Main.ParameterCount;

      public bool Matches(Signature signature)
      {
         RejectNull(Main, LOCATION, $"Main for {Name} not created");
         return Main.Matches(signature);
      }

      public bool ReturnNull { get; set; }

      public bool Initializer { get; set; }

      public IMaybe<ObjectRegion> ObjectRegion { get; set; } = none<ObjectRegion>();
   }
}