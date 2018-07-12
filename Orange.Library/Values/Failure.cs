using Orange.Library.Managers;

namespace Orange.Library.Values
{
   public class Failure : Value
   {
      InterpolatedString message;

      public Failure(InterpolatedString message) => this.message = message;

      public Failure(string text)
         : this(new InterpolatedString(text)) { }

      public override int Compare(Value value) => value is Failure f ? message.Compare(f.message) : -1;

      public override string Text
      {
         get => message.Text;
         set { }
      }

      public override double Number { get; set; }

      public override ValueType Type => ValueType.Failure;

      public override bool IsTrue => false;

      public override Value Clone() => new Failure(message);

      protected override void registerMessages(MessageManager manager)
      {
         manager.RegisterMessage(this, "map", v => ((Failure)v).Map());
         manager.RegisterMessage(this, "fmap", v => ((Failure)v).FlatMap());
         manager.RegisterMessage(this, "value", v => ((Failure)v).Value());
         manager.RegisterMessage(this, "else", v => ((Failure)v).Else());
      }

      public Value Map() => this;

      public Value FlatMap() => this;

      public Value Value() => message;

      public Value Else()
      {
         using (var assistant = new ParameterAssistant(Arguments))
         {
            var block = assistant.Block();
            if (block == null)
               return this;

            return new Some(block.Evaluate());
         }
      }

      public override string ToString() => $"#\"{message}\"";

      public (bool, string, Value) Match(Value right) => right is Failure f ? (Case.Match(message, f.message, false, null), "", message) :
         (false, "", (Value)null);
   }
}