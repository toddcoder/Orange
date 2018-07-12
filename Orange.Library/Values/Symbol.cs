using Orange.Library.Managers;
using Standard.Computer;
using static System.StringComparison;
using static Orange.Library.Managers.RegionManager;
using static Orange.Library.NewOrangeCompiler;
using static Orange.Library.Runtime;

namespace Orange.Library.Values
{
   public class Symbol : Value
   {
      const string LOCATION = "Symbol";

      string text;

      public Symbol(string text) => this.text = text;

      public Symbol()
         : this("") { }

      public override int Compare(Value value) => string.Compare(text, value.Text, Ordinal);

      public override string Text
      {
         get { return text; }
         set { }
      }

      public override double Number
      {
         get { return 0; }
         set { }
      }

      public override ValueType Type => ValueType.Symbol;

      public override bool IsTrue => true;

      public override Value Clone() => new Symbol(text);

      protected override void registerMessages(MessageManager manager)
      {
         manager.RegisterMessage(this, "remove", v => ((Symbol)v).Remove());
         manager.RegisterMessage(this, "maxOf", v => ((Symbol)v).MaxOf());
         manager.RegisterMessage(this, "minOf", v => ((Symbol)v).MinOf());
         manager.RegisterMessage(this, "breakOn", v => ((Symbol)v).BreakOn());
         manager.RegisterMessage(this, "isDef", v => ((Symbol)v).Exists());
         manager.RegisterMessage(this, "len", v => ((Symbol)v).Length());
         manager.RegisterMessage(this, "import", v => ((Symbol)v).Import());
         manager.RegisterMessage(this, "isLocal", v => ((Symbol)v).IsLocal());
      }

      public Value Import()
      {
         FileName file = @"C:\Enterprise\Modules\" + text + ".orange";
         Assert(file.Exists(), LOCATION, $"Module {text} doesn't exist");
         var source = file.Text;
         var block = Compile(source);
         var region = new Region();
         block.AutoRegister = false;
         State.RegisterBlock(block, region);
         block.Evaluate();
         State.UnregisterBlock();
         return new Module(region);
      }

      public Value Length() => text.Length;

      public Value Exists() => Regions.VariableExists(text);

      public Value BreakOn()
      {
         var value = Regions[text];
         var compare = Arguments[0];
         if (value.IsEmpty)
         {
            Regions[text] = compare;
            return false;
         }

         if (value.Compare(compare) == 0)
            return false;

         Regions[text] = compare;
         return true;
      }

      public Value Remove()
      {
         Regions.Current.Remove(text);
         return null;
      }

      public Value MaxOf()
      {
         var value = Regions[text];
         var compare = Arguments[0];
         if (value.IsEmpty)
         {
            Regions[text] = compare.Type == ValueType.Number ? compare.Number : compare;
            return Regions[text];
         }

         if (compare.Type == ValueType.Number)
         {
            if (compare.Number > value.Number)
               Regions[text] = compare.Number;
         }
         else
         {
            if (string.Compare(compare.Text, value.Text, Ordinal) > 0)
               Regions[text] = compare;
         }
         return Regions[text];
      }

      public Value MinOf()
      {
         var value = Regions[text];
         var compare = Arguments[0];
         if (value.IsEmpty)
         {
            Regions[text] = compare.Type == ValueType.Number ? compare.Number : compare;
            return Regions[text];
         }

         if (compare.Type == ValueType.Number)
         {
            if (compare.Number < value.Number)
               Regions[text] = compare.Number;
         }
         else
         {
            if (string.Compare(value.Text, compare.Text, Ordinal) < 0)
               Regions[text] = compare;
         }
         return Regions[text];
      }

      public override string ToString() => $"%{text}";

      public Value IsLocal() => Regions.Parent().Locals.ContainsKey(text);
   }
}