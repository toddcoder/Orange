using System;
using Orange.Library.Managers;
using Standard.Types.Strings;

namespace Orange.Library.Values
{
   public class Generate : Value
   {
      public override int Compare(Value value) => value is Generate ? 0 : -1;

      public override string Text { get; set; } = "";

      public override double Number { get; set; }

      public override ValueType Type => ValueType.Generate;

      public override bool IsTrue => true;

      public override Value Clone() => new Generate();

      protected override void registerMessages(MessageManager manager)
      {
         manager.RegisterMessage(this, "now", v => Now());
         manager.RegisterMessage(this, "today", v => Today());
         manager.RegisterMessage(this, "guid", v => GUID());
         manager.RegisterMessage(this, "uid", v => UniqueID());
      }

      public static Value Now() => new Date(DateTime.Now);

      public static Value Today() => new Date(DateTime.Today);

      public static Value GUID() => new String(Guid.NewGuid().ToString());

      public static Value UniqueID() => StringFunctions.UniqueID();
   }
}