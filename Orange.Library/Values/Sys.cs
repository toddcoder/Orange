using Orange.Library.Managers;
using Orange.Library.Messages;
using Standard.Types.Collections;
using static System.Guid;
using static Orange.Library.Managers.RegionManager;
using static Orange.Library.Runtime;
using static Standard.Types.Strings.StringFunctions;

namespace Orange.Library.Values
{
   public class Sys : Value, IMessageHandler
   {
      public override int Compare(Value value) => 0;

      public override string Text
      {
         get => "sys";
         set { }
      }

      public override double Number { get; set; }

      public override ValueType Type => ValueType.System;

      public override bool IsTrue => false;

      public override Value Clone() => this;

      protected override void registerMessages(MessageManager manager)
      {
         manager.RegisterMessageCall("dump");
         manager.RegisterMessage(this, "dump", v => Regions.Dump());
         manager.RegisterMessage(this, "exit", v => Exit());
         manager.RegisterMessage(this, "skip", v => Skip());
         manager.RegisterMessage(this, "uid", v => UID());
         manager.RegisterMessage(this, "guid", v => GUID());
         manager.RegisterMessage(this, "take", v => Take());
         manager.RegisterMessage(this, "binding", v => Binding());
         manager.RegisterMessage(this, "allVars", v => AllVariables());
         manager.RegisterMessage(this, "context", v => ((Sys)v).Context());
      }

      public Value Context()
      {
         if (Arguments.Executable.CanExecute)
            Arguments.Executable.Evaluate();
         return null;
      }

      public static Value Binding() => new Binding();

      public static Value Take() => State.Take();

      public static Value UID() => UniqueID();

      public static Value GUID() => NewGuid().ToString();

      public static Value Exit()
      {
         State.ExitSignal = true;
         return null;
      }

      public static Value Skip()
      {
         State.SkipSignal = true;
         return null;
      }

      public override string ToString() => "sys";

      public static Value AllVariables()
      {
         var variables = Regions.Current.AllVariables();
         variables.Remove("sys");
         return new Array(variables.ToAutoHash(k => new Nil()));
      }

      public Value Send(Value value, string messageName, Arguments arguments, out bool handled)
      {
         if (Regions.VariableExists(messageName) && Regions[messageName] is IInvokeable invokeable)
         {
            handled = true;
            return invokeable.Invoke(arguments);
         }

         handled = false;
         return null;
      }

      public bool RespondsTo(string messageName) => Regions.VariableExists(messageName);
   }
}