using System;
using Orange.Library.Managers;
using static System.StringComparison;
using static Orange.Library.Values.Nil;

namespace Orange.Library.Values
{
   public class Error : Value
   {
      InterpolatedString message;

      public Error(InterpolatedString message) => this.message = message;

      public override int Compare(Value value) => string.Compare(Text, value.Text, Ordinal);

      public override string Text
      {
         get { return message.Text; }
         set { }
      }

      public override double Number
      {
         get { return 0; }
         set { }
      }

      public override ValueType Type => ValueType.Error;

      public override bool IsTrue => false;

      public override Value Clone() => new Error(message);

      protected override void registerMessages(MessageManager manager)
      {
         manager.RegisterMessage(this, "assert", v => ((Error)v).Assert());
         manager.RegisterMessage(this, "reject", v => ((Error)v).Reject());
         manager.RegisterMessage(this, "throw", v => ((Error)v).Throw());
      }

      public Value Assert()
      {
         var value = Arguments[0];
         if (!value.IsTrue)
            throw new ApplicationException(message.Text);

         return NilValue;
      }

      public Value Reject()
      {
         var value = Arguments[0];
         if (value.IsTrue)
            throw new ApplicationException(message.Text);

         return NilValue;
      }

      public Value Throw() => throw new ApplicationException(message.Text);

      public override string ToString() => $@"!""{message.Text.Replace(@"""", @"`""")}""";
   }
}