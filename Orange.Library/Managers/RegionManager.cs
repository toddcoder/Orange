using System;
using System.Collections.Generic;
using System.Linq;
using Orange.Library.Values;
using Standard.Types.Collections;
using Standard.Types.Enumerables;
using Standard.Types.Maybe;
using Standard.Types.Strings;
using static System.DateTime;
using static System.Diagnostics.Debug;
using static System.Math;
using static Orange.Library.Compiler;
using static Orange.Library.Runtime;
using static Orange.Library.Values.Nil;
using static Standard.Types.Maybe.MaybeFunctions;
using static Standard.Types.Strings.StringFunctions;
using Array = Orange.Library.Values.Array;
using Double = Orange.Library.Values.Double;
using Object = Orange.Library.Values.Object;
using String = Orange.Library.Values.String;

namespace Orange.Library.Managers
{
   public class RegionManager
   {
      public static RegionManager Regions { get; set; }

      protected static StringSet specialVariables;

      static RegionManager() => specialVariables = new StringSet
      {
         "$out",
         "$rout",
         "$put",
         "$write",
         "$recs",
         "$fields",
         "$outb",
         "$putb",
         "$writeb",
         "$recsb",
         "$fieldsb",
         "$output",
         "$trace",
         "$now",
         "$today",
         "$ans",
         "$buff",
         "$id",
         "$uid",
         "$guid",
         "$root",
         "$seed",
         "_",
         "$in",
         "$fs",
         "$fp",
         "$rs",
         "$rp"
      };

      public static bool IsSpecialVariable(string variableName) => specialVariables.Contains(variableName);

      static Value print(Value[] values)
      {
         switch (values.Length)
         {
            case 0:
               return null;
            case 1:
               var asString = ValueAsString(values[0]);
               State.ConsoleManager.ConsolePrint(asString);
               return null;
            default:
               var text = values.Select(ValueAsString).Listify("");
               State.ConsoleManager.ConsolePrint(text);
               return null;
         }
      }

      static Value write(Value[] values)
      {
         switch (values.Length)
         {
            case 0:
               return null;
            case 1:
               var asString = ValueAsRep(values[0]);
               State.ConsoleManager.ConsolePrint(asString);
               return null;
            default:
               var text = values.Select(ValueAsRep).Listify("");
               State.ConsoleManager.ConsolePrint(text);
               return null;
         }
      }

      static Value println(Value[] values)
      {
         switch (values.Length)
         {
            case 0:
               State.ConsoleManager.ConsolePrintln("");
               return null;
            case 1:
               var asString = ValueAsString(values[0]);
               State.ConsoleManager.ConsolePrintln(asString);
               return null;
            default:
               var text = values.Select(ValueAsString).Listify("");
               State.ConsoleManager.ConsolePrintln(text);
               return null;
         }
      }

      static Value writeln(Value[] values)
      {
         switch (values.Length)
         {
            case 0:
               State.ConsoleManager.ConsolePrintln("");
               return null;
            case 1:
               var asString = ValueAsRep(values[0]);
               State.ConsoleManager.ConsolePrintln(asString);
               return null;
            default:
               var text = values.Select(ValueAsRep).Listify("");
               State.ConsoleManager.ConsolePrintln(text);
               return null;
         }
      }

      static Value put(Value[] values)
      {
         foreach (var value in values)
            State.ConsoleManager.Put(ValueAsString(value));

         return null; //values.Listify(State.FieldSeparator.Text);
      }

      static Value peek(Value[] values)
      {
         switch (values.Length)
         {
            case 1:
               State.ConsoleManager.ConsolePrintln(ValueAsString(values[0]));
               return values[0];
            case 2:
               State.ConsoleManager.ConsolePrintln(ValueAsString(values[0]));
               return values[1];
            default:
               return NilValue;
         }
      }

      static Value tabs(Value[] values)
      {
         switch (values.Length)
         {
            case 0:
               return "\t";
            default:
               return "\t".Repeat(values[0].Int);
         }
      }

      static Value invokeDouble(Value[] values, Func<Double, Value> func)
      {
         Runtime.Assert(values.Length >= 1, LOCATION, "Native function missing required parameter");
         if (values[0] is Double value)
            return func(value);

         Throw(LOCATION, $"{values[0]} isn't a Number");
         return null;
      }

      static Value isArray(Value[] values) => values[0].Type == Value.ValueType.Array;

      static Value now() => new Date(Now);

      static Value today() => new Date(Today);

      static Value time() => new Date((Now - Today).Ticks);

      const string LOCATION = "Region manager";

      Region[] regions;
      int level;

      public RegionManager(Sys sys, string text)
      {
         regions = new Region[MAX_VAR_DEPTH];
         level = 0;
         var head = new Region { Level = level };
         regions[level] = head;
         SetGlobals(sys, text, head);

         WriteLine("[g");
      }

      public int Level => level;

      public static void SetGlobals(Sys sys, string text, Region head)
      {
         head.CreateAndSet("sys", sys);
         head.CreateAndSet("$", text);
         head.CreateAndSet("pi", PI);
         head.CreateAndSet("exp", E);
         head.CreateAndSet("tab", "\t");
         head.CreateAndSet("id", CodeBuilder.Id());
         head.CreateAndSet("fprintln", CodeBuilder.FPrintln());
         head.CreateAndSet("fprint", CodeBuilder.FPrint());
         head.CreateAndSet("fput", CodeBuilder.FPut());
         head.CreateAndSet("degToRad", PI / 180);
         head.CreateAndSet("global", new Global());
         head.CreateAndSet("consts", new Consts());
         head.CreateAndSet("gen", new Generate());
         head.CreateAndSet("print", new NativeFunction("print", print));
         head.CreateAndSet("println", new NativeFunction("println", println));
         head.CreateAndSet("manif", new NativeFunction("manif", write));
         head.CreateAndSet("manifln", new NativeFunction("manifln", writeln));
         head.CreateAndSet("put", new NativeFunction("put", put));
         head.CreateAndSet("peek", new NativeFunction("peek", peek));
         head.CreateAndSet("isArray", new NativeFunction("isArray", isArray));
         head.CreateAndSet("sqrt", new NativeFunction("sqrt", v => invokeDouble(v, d => d.Sqrt())));
         head.CreateAndSet("abs", new NativeFunction("abs", v => invokeDouble(v, d => Abs(d.Number))));
         head.CreateAndSet("acos", new NativeFunction("acos", v => invokeDouble(v, d => Acos(d.Number))));
         head.CreateAndSet("asin", new NativeFunction("asin", v => invokeDouble(v, d => Asin(d.Number))));
         head.CreateAndSet("atan", new NativeFunction("atan", v => invokeDouble(v, d => Atan(d.Number))));
         head.CreateAndSet("cos", new NativeFunction("cos", v => invokeDouble(v, d => Cos(d.Number))));
         head.CreateAndSet("sin", new NativeFunction("sin", v => invokeDouble(v, d => Sin(d.Number))));
         head.CreateAndSet("tan", new NativeFunction("tan", v => invokeDouble(v, d => Tan(d.Number))));
         head.CreateAndSet("log", new NativeFunction("log", v => invokeDouble(v, d => Log10(d.Number))));
         head.CreateAndSet("ln", new NativeFunction("ln", v => invokeDouble(v, d => Log(d.Number))));
         head.CreateAndSet("succ", new NativeFunction("succ", v => invokeDouble(v, d => d.Number + 1)));
         head.CreateAndSet("pred", new NativeFunction("pred", v => invokeDouble(v, d => d.Number - 1)));
         head.CreateAndSet("Buffer", new NativeFunction("Buffer", v => new Values.Buffer()));
         head.CreateAndSet("now", new NativeFunction("now", v => now()));
         head.CreateAndSet("today", new NativeFunction("today", v => today()));
         head.CreateAndSet("time", new NativeFunction("time", v => time()));
         head.CreateAndSet("tabs", new NativeFunction("tabs", tabs));
         head.CreateAndSet("indentation", new NativeFunction("indentation", v => State.Indentation()));
      }

      public void Reset()
      {
         var sys = (Sys)regions[0]["sys"];
         var text = regions[0]["$"].Text;
         var head = new Region { Level = 0 };
         regions[0] = head;
         SetGlobals(sys, text, head);
      }

      public bool Field(string fieldName, out Value value)
      {
         if (specialVariables.Contains(fieldName))
         {
            value = getSpecialField(fieldName);
            return true;
         }

         value = null;
         for (var i = level; i >= 0; i--)
         {
            var region = regions[i];
            value = region[fieldName];
            if (value != null)
               return true;
         }

         Throw(LOCATION, $"Field {fieldName} not defined");
         return false;
      }

      public bool FieldExists(string fieldName)
      {
         if (specialVariables.Contains(fieldName))
            return true;

         for (var i = level; i >= 0; i--)
         {
            var region = regions[i];
            if (region.ContainsMessage(fieldName))
               return true;
         }

         return false;
      }

      public void SetField(string fieldName, Value value, bool _override = false)
      {
         if (specialVariables.Contains(fieldName))
         {
            setSpecialField(fieldName, value);
            return;
         }

         for (var i = level; i >= 0; i--)
         {
            var region = regions[i];
            if (region.ContainsMessage(fieldName))
            {
               region[fieldName] = value;
               return;
            }
         }

         Throw(LOCATION, $"Field {fieldName} not defined");
      }

      public void SetOrCreateField(string fieldName, Value value)
      {
         if (specialVariables.Contains(fieldName))
         {
            setSpecialField(fieldName, value);
            return;
         }

         for (var i = level; i >= 0; i--)
         {
            var region = regions[i];
            if (region.ContainsMessage(fieldName))
            {
               region[fieldName] = value;
               return;
            }
         }

         Regions.Current.CreateAndSet(fieldName, value);
      }

      public void SetBinding(string fieldName, Value value, bool assigning)
      {
         if (assigning)
            SetOrCreateField(fieldName, value);
         else
            SetLocal(fieldName, value);
      }

      public void RemoveField(string fieldName)
      {
         Reject(specialVariables.Contains(fieldName), LOCATION, $"Special field {fieldName} can't be removed");
         for (var i = level; i >= 0; i--)
         {
            var region = regions[i];
            if (region.ContainsMessage(fieldName))
            {
               region.Remove(fieldName);
               return;
            }
         }

         Throw(LOCATION, $"Field {fieldName} not defined");
      }

      public bool FieldIsReadOnly(string fieldName)
      {
         for (var i = level; i >= 0; i--)
         {
            var region = regions[i];
            if (region.ContainsMessage(fieldName))
               return region.IsReadOnly(fieldName);
         }

         Throw(LOCATION, $"Field {fieldName} not defined");
         return false;
      }

      static Value getSpecialField(string name)
      {
         switch (name)
         {
            case "$out":
            case "$rout":
            case "$put":
            case "$write":
               return State.ConsoleManager.LastOutput;
            case "$recs":
            case "$fields":
            case "$outb":
            case "$putb":
            case "$writeb":
            case "$recsb":
            case "$fieldsb":
               return new Nil();
            case "$output":
               return State.PrintBuffer;
            case "$trace":
               return State.Tracing;
            case "$now":
               return Now;
            case "$today":
               return Today;
            case "$ans":
               var result = State.Result;
               return result ?? "";
            case "$buff":
               return State.Buffer;
            case "$id":
               return CompilerState.ObjectID();
            case "$uid":
               return UniqueID();
            case "$guid":
               return GUID();
            case "$root":
               return new Graph("$root", "");
            case "$seed":
               return State.Seed;
            case "_":
               return new Any();
            case "$in":
               return State.UIConsole?.Read();
            case "$fs":
               return State.FieldSeparator;
            case "$fp":
               return State.FieldPattern;
            case "$rs":
               return State.RecordSeparator;
            case "$rp":
               return State.RecordPattern;
            default:
               return null;
         }
      }

      public static string ValueAsString(Value value)
      {
         switch (value)
         {
            case Object obj when obj.RespondsNoDefault("str"):
               return SendMessage(obj, "str").Text;
            case INSGenerator generator:
               return ToArray(generator).Text;
            default:
               return value.Text;
         }
      }

      public static string ValueAsRep(Value value)
      {
         switch (value)
         {
            case Object obj when obj.RespondsNoDefault("str"):
               return SendMessage(obj, "str").ToString();
            case INSGenerator generator:
               return ToArray(generator).ToString();
            default:
               return value.ToString();
         }
      }

      static void iterate(Value value, Action<string> action)
      {
         if (value.IsArray)
            foreach (var item in (Array)value.SourceArray)
               action(ValueAsString(item.Value));
         else if (value.Type == Value.ValueType.Iterator)
            foreach (var result in (IEnumerable<Value>)value)
               action(ValueAsString(result));
         else
            action(ValueAsString(value));
      }

      static void setSpecialField(string name, Value value)
      {
         switch (name)
         {
            case "$out":
               State.ConsoleManager.Print(ValueAsString(value));
               return;
            case "$outb":
               State.BuffPrint(ValueAsString(value));
               return;
            case "$rout":
               State.ConsoleManager.Print(value.ToString());
               return;
            case "$put":
               State.ConsoleManager.Put(ValueAsString(value));
               return;
            case "$putb":
               State.BuffPut(ValueAsString(value));
               return;
            case "$write":
               State.ConsoleManager.Write(ValueAsString(value));
               return;
            case "$writeb":
               State.BuffWrite(ValueAsString(value));
               return;
            case "$recs":
               iterate(value, v => State.ConsoleManager.Print(v));
               return;
            case "$recsb":
               iterate(value, v => State.BuffPrint(v));
               return;
            case "$fields":
               iterate(value, v => State.ConsoleManager.Put(v));
               return;
            case "$fieldsb":
               iterate(value, v => State.BuffPut(v));
               return;
            case "_":
            case "$output":
            case "$now":
            case "$today":
            case "$id":
            case "$uid":
            case "$guid":
            case "$in":
               return;
            case "$trace":
               State.Tracing = value.IsTrue;
               return;
            case "$ans":
               State.Result = value;
               return;
            case "$seed":
               State.Seed = value;
               return;
            case "$fs":
               State.FieldSeparator = new String(value.Text);
               return;
            case "$fp":
            {
               State.FieldPattern = value is Pattern p ? p : new Pattern();
               return;
            }
            case "$rs":
               State.RecordSeparator = new String(value.Text);
               return;
            case "$rp":
            {
               State.RecordPattern = value is Pattern p ? p : new Pattern();
               return;
            }
         }
      }

      public Value this[string name]
      {
         get
         {
            WriteLine($"getting {name} in {name}");

            Reject(name.IsEmpty(), LOCATION, "Name zero length (getting)");

            Value value;
            if (Field(name, out value))
               return value;

            return null;
         }
         set
         {
            Runtime.Assert(name.IsNotEmpty(), LOCATION, "Name zero length (setting)");
            if (value == null || value.IsNil)
               return;

            SetField(name, value);
         }
      }

      public Region Global => regions[0];

      public void Dispose()
      {
         for (var i = level; i >= 0; i--)
            regions[i].Dispose();
      }

      public Region Current => regions[level];

      public void Push(string tag)
      {
         Runtime.Assert(Count < MAX_VAR_DEPTH, LOCATION, "Regions nested too deeply");
         var region = new Region
         {
            Tag = tag,
            Level = ++level
         };
         regions[level] = region;
         //WriteLine($"{"-".Repeat(level)}[{tag}={region.Name}->{regions[level - 1].Name}");
      }

      public void Push(Region region, string tag)
      {
         Runtime.Assert(Count < MAX_VAR_DEPTH, LOCATION, "Regions nested too deeply");
         region.Tag = tag;
         region.Level = ++level;
         regions[level] = region;
         //WriteLine($"{"-".Repeat(level)}[{tag}={region.Name}->{regions[level - 1].Name}");
      }

      public void Pop(string text)
      {
         Runtime.Assert(Count > 0, LOCATION, "Regions popped unevenly");
         var region = regions[level--];
         region.Dispose();
         WriteLine($"{"-".Repeat(level)}]{text}");
      }

      public string Dump() => Current.Dump();

      public int Count => level + 1;

      public void SetLocal(string name, Value value, Object.VisibilityType visibility = Object.VisibilityType.Public,
         bool _override = false, bool allowNil = false)
      {
         Current.SetLocal(name, value, visibility, _override, allowNil);
      }

      public void SetParameter(string name, Value value, Object.VisibilityType visibility = Object.VisibilityType.Public,
         bool _override = false, bool allowNil = false)
      {
         Current.SetParameter(name, value, visibility, _override, allowNil);
      }

      public void SetReadOnly(string name) => Current.SetReadOnly(name);

      public void UnsetReadOnly(string name) => Current.UnsetReadOnly(name);

      public void RemoveVariable(string name) => RemoveField(name);

      public bool VariableExists(string name) => FieldExists(name);

      public void CreateVariable(string variableName, bool global = false,
         Object.VisibilityType visibility = Object.VisibilityType.Public, bool _override = false)
      {
         Current.CreateVariable(variableName, global, visibility, _override);
      }

      public void CreateVariableIfNonexistant(string variableName, bool global = false,
         Object.VisibilityType visibility = Object.VisibilityType.Public, bool _override = false)
      {
         Current.CreateVariableIfNonexistant(variableName, global, visibility, _override);
      }

      public void CreateReadOnlyVariable(string variableName, bool global = false,
         Object.VisibilityType visibility = Object.VisibilityType.Public, bool _override = false)
      {
         Current.CreateReadOnlyVariable(variableName, global, visibility, _override);
      }

      public IMaybe<Value> ValueFromVariable(string name) => when(VariableExists(name), () => this[name]);

      public bool IsReadOnly(string variableName) => FieldIsReadOnly(variableName);

      public void SetPatternPositionData(int position, int length)
      {
         SetLocal(VAR_AT, position);
         SetLocal(VAR_LENGTH, length);
      }

      public Region Parent() => Parent(level);

      public Region Parent(int forLevel) => forLevel <= 0 ? null : regions[forLevel - 1];

      public Region GrandParent() => GrandParent(level);

      public Region GrandParent(int forLevel) => Parent(forLevel - 1);

      public void ForEachRegion(Action<Region> action)
      {
         for (var i = level; i >= 0; i--)
            action(regions[i]);
      }

      public Region RegionAtLevel(int requestedLevel) => regions[requestedLevel];

      public void Clear() => ForEachRegion(region => region.RemoveAll());
   }
}