using static Orange.Library.Runtime;

namespace Orange.Library.Values
{
   public class ObjectPropertyVariable2 : Variable
   {
      const string LOCATION = "Property";

      Object obj;

      public ObjectPropertyVariable2(Object obj, string name)
         : base(name) => this.obj = obj;

      public IInvokeable Getter { get; set; }

      public IInvokeable Setter { get; set; }

      public IInvokeable Before { get; set; }

      public IInvokeable After { get; set; }

      public IInvokeable Invariant { get; set; }

      public override Value Value
      {
         get
         {
            assertGetter();
            return obj.Invoke(Getter, new Arguments());
         }
         set
         {
            assertSetter();

            var hasAfter = After != null;
            var hasInvariant = Invariant != null;
            var eitherEnding = hasAfter || hasInvariant;
            Value oldValue = new Nil();

            if (eitherEnding)
            {
               assertGetter();
               oldValue = obj.Invoke(Getter, new Arguments());
            }

            var valueArguments = new Arguments(value);

            if (Before != null)
               obj.Invoke(Before, valueArguments);

            obj.Invoke(Setter, valueArguments);

            if (!eitherEnding)
               return;

            var valuesArguments = new Arguments();
            valuesArguments.AddArgument(oldValue);
            valuesArguments.AddArgument(value);

            if (hasAfter)
               obj.Invoke(After, valuesArguments);

            if (!hasInvariant)
               return;

            var result = obj.Invoke(Invariant, valuesArguments);
            Assert(result.IsTrue, LOCATION, $"Invariant for {Name} failed");
         }
      }

      void assertGetter() => RejectNull(Getter, LOCATION, $"No getter for {Name}");

      void assertSetter() => RejectNull(Setter, LOCATION, $"No setter for {Name}");

      public override ValueType Type => ValueType.Lambda;

/*	   public override string ToString()
		{
			try
			{
				return Value.ToString();
			}
			catch
			{
				return Name;
			}
		}*/
      public override string ToString() => Name; //Value.ToString();
   }
}