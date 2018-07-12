using System.Collections.Generic;
using System.Linq;
using Orange.Library.Managers;
using Standard.Types.Collections;
using static Orange.Library.Managers.MessageManager;

namespace Orange.Library.Values
{
   public class MessageSet : Value
   {
      Set<string> messages;

      public MessageSet() => messages = new Set<string>();

      public MessageSet(IEnumerable<string> messages)
         : this()
      {
         this.messages.AddRange(messages);
      }

      public override int Compare(Value value) => messages.All(msg => MessagingState.RespondsTo(value, msg)) ? 0 : -1;

      public override string Text
      {
         get;
         set;
      } = "";

      public override double Number
      {
         get;
         set;
      }

      public override ValueType Type => ValueType.MessageSet;

      public override bool IsTrue => false;

      public override Value Clone() => new MessageSet();

      protected override void registerMessages(MessageManager manager)
      {
      }
   }
}