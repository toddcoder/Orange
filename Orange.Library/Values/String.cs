using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Orange.Library.Managers;
using Orange.Library.Parsers;
using Standard.Computer;
using Standard.Internet.XML;
using Standard.Types.Arrays;
using Standard.Types.Collections;
using Standard.Types.Enumerables;
using Standard.Types.Maybe;
using Standard.Types.Numbers;
using Standard.Types.RegularExpressions;
using Standard.Types.Strings;
using static System.StringComparison;
using static System.Text.Encoding;
using static Orange.Library.Runtime;
using static Orange.Library.Managers.RegionManager;
using static Orange.Library.ParameterAssistant;
using static Orange.Library.ParameterAssistant.SignalType;
using static Orange.Library.Managers.MessageManager;
using static Orange.Library.Parsers.StatementParser;
using static Orange.Library.Values.Nil;
using static Standard.Types.Booleans.Assertions;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Orange.Library.Values
{
   public class String : Value, IRepeatable, INSGeneratorSource
   {
      const string LOCATION = "String";
      const string REGEX_ALPHA = "^ {A-Za-z} $";
      const string REGEX_ALPHANUM = "^ {A-Za-z0-9} $";

      public static string Pad(int width, string padding)
      {
         var length = padding.Length;
         if (width < length)
            return padding.Substring(0, width);

         var count = width / length;
         var remainder = width - length * count;
         return padding.Repeat(count) + padding.Substring(0, remainder);
      }

      public static string PadLeft(string text, int width, string padding)
      {
         var remainingWidth = width - text.Length;
         if (remainingWidth <= 0)
            return text;

         return Pad(remainingWidth, padding) + text;
      }

      public static string PadRight(string text, int width, string padding)
      {
         var remainingWidth = width - text.Length;
         if (remainingWidth <= 0)
            return text;

         return text + Pad(remainingWidth, padding);
      }

      public static string PadCenter(string text, int width, string padding)
      {
         var remainingWidth = width - text.Length;
         int half;
         if (remainingWidth <= 0)
         {
            var extra = text.Length - width;
            half = extra / 2;
            return text.Substring(half, width);
         }

         half = remainingWidth / 2;
         var padLeft = half;
         var padRight = remainingWidth.IsEven() ? half : half + 1;
         return Pad(padLeft, padding) + text + Pad(padRight, padding);
      }

      protected string text;

      public String(string text)
      {
         RejectNull(text, LOCATION, "String not set");
         this.text = text;
      }

      public String()
         : this("") { }

      protected virtual string getText()
      {
         RejectNull(text, LOCATION, "String not set");
         return text;
      }

      public override int Compare(Value value)
      {
         if (id == value.ID)
            return 0;

         if (value is String other)
            return string.Compare(getText(), other.text, Ordinal);

         return -1;
      }

      public override string Text
      {
         get => getText();
         set => text = value;
      }

      public override double Number
      {
         get => ConvertStringToNumber(this).Number;
         set => text = value.ToString();
      }

      public override Value Do(int count) => Text.Repeat(count);

      public virtual Value Query
      {
         get
         {
            FileName file = getText();
            Assert(file.Exists(), $"File {file} doesn't exist");
            return file.Text;
         }
      }

      protected override void registerMessages(MessageManager manager)
      {
         manager.RegisterMessage(this, "len", v => v.Text.Length);
         manager.RegisterMessage(this, "repeat", v => ((String)v).Repeat());
         manager.RegisterMessage(this, "eq", v => ((String)v).Same());
         manager.RegisterMessage(this, "neq", v => ((String)v).NotSame());
         manager.RegisterMessage(this, "rev", v => ((String)v).Reverse());
         manager.RegisterMessage(this, "strip", v => ((String)v).Strip());
         manager.RegisterMessage(this, "lstrip", v => ((String)v).LStrip());
         manager.RegisterMessage(this, "rstrip", v => ((String)v).RStrip());
         manager.RegisterMessage(this, "in", v => ((String)v).In());
         manager.RegisterMessage(this, "notIn", v => ((String)v).NotIn());
         manager.RegisterMessage(this, "recs", v => ((String)v).Recs(), false);
         manager.RegisterMessage(this, "dref", v => ((String)v).Ref());
         manager.RegisterMessage(this, "chars", v => ((String)v).Chars());
         manager.RegisterMessage(this, "rs", v => ((String)v).SetRecordSeparator());
         manager.RegisterMessage(this, "fs", v => ((String)v).SetFieldSeparator());
         manager.RegisterMessage(this, "fields", v => ((String)v).Fields());
         manager.RegisterMessage(this, "to", v => ((String)v).To());
         manager.RegisterMessage(this, "over", v => ((String)v).Over());
         manager.RegisterMessage(this, "upper", v => ((String)v).Upper());
         manager.RegisterMessage(this, "lower", v => ((String)v).Lower());
         manager.RegisterMessage(this, "capital", v => ((String)v).Capital());
         manager.RegisterMessage(this, "leg", v => ((String)v).Comparison());
         manager.RegisterMessage(this, "has", v => ((String)v).Has());
         manager.RegisterMessage(this, "ord", v => ((String)v).Ord());
         manager.RegisterMessage(this, "center", v => ((String)v).Center());
         manager.RegisterMessage(this, "rjust", v => ((String)v).PadLeft());
         manager.RegisterMessage(this, "ljust", v => ((String)v).PadRight());
         manager.RegisterMessage(this, "count", v => ((String)v).Count());
         manager.RegisterMessage(this, "seek", v => ((String)v).Seek());
         manager.RegisterMessage(this, "seekLast", v => ((String)v).SeekLast());
         manager.RegisterMessage(this, "isPrefix", v => ((String)v).StartsWith());
         manager.RegisterMessage(this, "isSuffix", v => ((String)v).EndsWith());
         manager.RegisterMessage(this, "isAlpha", v => ((String)v).IsAlpha());
         manager.RegisterMessage(this, "isAlphan", v => ((String)v).IsAlphaNum());
         manager.RegisterMessage(this, "isUpper", v => ((String)v).IsUpper());
         manager.RegisterMessage(this, "isLower", v => ((String)v).IsLower());
         manager.RegisterMessage(this, "isDigit", v => ((String)v).IsDigit());
         manager.RegisterMessage(this, "isVowel", v => ((String)v).IsVowel());
         manager.RegisterMessage(this, "isCons", v => ((String)v).IsConsonant());
         manager.RegisterMessage(this, "max", v => ((String)v).Max());
         manager.RegisterMessage(this, "min", v => ((String)v).Min());
         manager.RegisterMessage(this, "end", v => v.Text.Length - 1);
         manager.RegisterMessage(this, "squote", v => ((String)v).SQuote());
         manager.RegisterMessage(this, "dquote", v => ((String)v).DQuote());
         manager.RegisterMessage(this, "mc", v => ((String)v).MatchCase());
         manager.RegisterMessage(this, "title", v => ((String)v).Title());
         manager.RegisterMessage(this, "charmap", v => ((String)v).CharMap(false));
         manager.RegisterMessage(this, "charmapd", v => ((String)v).CharMap(true));
         manager.RegisterMessage(this, "charmapc", v => ((String)v).CharMapC());
         manager.RegisterMessage(this, "charmaps", v => ((String)v).CharMapS());
         manager.RegisterMessage(this, "eval", v => ((String)v).Evaluate());
         manager.RegisterMessage(this, "sub", v => ((String)v).Substring());
         manager.RegisterMessage(this, "replace", v => ((String)v).Replace());
         manager.RegisterMessage(this, "replaceAll", v => ((String)v).ReplaceAll());
         manager.RegisterMessage(this, "left", v => ((String)v).Left());
         manager.RegisterMessage(this, "right", v => ((String)v).Right());
         manager.RegisterMessage(this, "field", v => ((String)v).Field());
         manager.RegisterMessage(this, "insert", v => ((String)v).Insert());
         manager.RegisterMessage(this, "overlay", v => ((String)v).Overlay());
         manager.RegisterMessage(this, "del", v => ((String)v).Delete());
         manager.RegisterMessage(this, "delField", v => ((String)v).DeleteField());
         manager.RegisterMessage(this, "space", v => ((String)v).Space());
         manager.RegisterMessage(this, "find", v => ((String)v).Find());
         manager.RegisterMessage(this, "findBack", v => ((String)v).FindBack());
         manager.RegisterMessage(this, "indexes", v => ((String)v).Indexes());
         manager.RegisterMessage(this, "lastIndex", v => ((String)v).LastIndexOf());
         manager.RegisterMessage(this, "cmd", v => ((String)v).Command());
         manager.RegisterMessage(this, "compare", v => ((String)v).Compare());
         manager.RegisterMessage(this, "compareTo", v => ((String)v).CompareTo());
         manager.RegisterMessage(this, "findField", v => ((String)v).FindFIeld());
         manager.RegisterMessage(this, "justify", v => ((String)v).Justify());
         manager.RegisterMessage(this, "expand", v => ((String)v).Expand());
         manager.RegisterMessage(this, "succ", v => ((String)v).Succ());
         manager.RegisterMessage(this, "pred", v => ((String)v).Pred());
         manager.RegisterMessage(this, "addRec", v => ((String)v).AddRecord());
         manager.RegisterMessage(this, "addField", v => ((String)v).AddField());
         manager.RegisterMessage(this, "first", v => ((String)v).First());
         manager.RegisterMessage(this, "last", v => ((String)v).Last());
         manager.RegisterMessage(this, "notFirst", v => ((String)v).NotFirst());
         manager.RegisterMessage(this, "notLast", v => ((String)v).NotLast());
         manager.RegisterMessage(this, "mid", v => ((String)v).Middle());
         manager.RegisterMessage(this, "notMid", v => ((String)v).NotMiddle());
         manager.RegisterMessage(this, "where", v => ((String)v).Where());
         manager.RegisterMessage(this, "split", v => ((String)v).Split());
         manager.RegisterMessage(this, "visible", v => ((String)v).Visible());
         manager.RegisterMessage(this, "eachChr", v => ((String)v).Each());
         manager.RegisterMessage(this, "squeeze", v => ((String)v).Squeeze());
         manager.RegisterMessage(this, "invCase", v => ((String)v).InvertCase());
         manager.RegisterMessage(this, "toBase64", v => ((String)v).ToBase64());
         manager.RegisterMessage(this, "fromBase64", v => ((String)v).FromBase64());
         manager.RegisterMessage(this, "xtidy", v => ((String)v).XTidy());
         manager.RegisterMessage(this, "xmlify", v => ((String)v).XMLify());
         manager.RegisterMessage(this, "unxmlify", v => ((String)v).UnXMLify());
         manager.RegisterMessage(this, "sprint", v => ((String)v).SPrint());
         manager.RegisterMessage(this, "sput", v => ((String)v).SPut());
         manager.RegisterMessage(this, "swrite", v => ((String)v).SWrite());
         manager.RegisterMessage(this, "cube", v => ((String)v).Cube());
         manager.RegisterMessage(this, "add", v => ((String)v).Push());
         manager.RegisterMessage(this, "unadd", v => ((String)v).Pop());
         manager.RegisterMessage(this, "top", v => ((String)v).Unshift());
         manager.RegisterMessage(this, "read", v => ((String)v).Shift());
         manager.RegisterMessage(this, "trunc", v => ((String)v).Truncate());
         manager.RegisterMessage(this, "ask", v => Ask());
         manager.RegisterMessage(this, "get", v => ((String)v).Get());
         manager.RegisterMessage(this, "frecs", v => ((String)v).FRecs());
         manager.RegisterMessage(this, "lines", v => ((String)v).Lines());
         manager.RegisterMessage(this, "flines", v => ((String)v).FLines());
         manager.RegisterMessage(this, "append", v => ((String)v).Append());
         manager.RegisterMessage(this, "trans", v => ((String)v).Transpose());
         manager.RegisterMessage(this, "assign", v => ((String)v).Assign());
         manager.RegisterMessage(this, "xml", v => ((String)v).XML());
         manager.RegisterMessage(this, "expandTabs", v => ((String)v).ExpandTabs());
         manager.RegisterMessage(this, "sym", v => ((String)v).Symbol());
         manager.RegisterMessage(this, "unstring", v => ((String)v).Unstring());
         manager.RegisterMessage(this, "json", v => v.ToString());
         manager.RegisterMessage(this, "urlEncode", v => ((String)v).URLEncode());
         manager.RegisterMessage(this, "urlDecode", v => ((String)v).URLDecode());
         manager.RegisterMessage(this, "isNumeric", v => v.IsNumeric());
         manager.RegisterMessage(this, "isSpace", v => ((String)v).IsSpace());
         manager.RegisterMessage(this, "isTitle", v => ((String)v).IsTitle());
         manager.RegisterMessage(this, "swapCase", v => ((String)v).SwapCase());
         manager.RegisterMessage(this, "assembly", v => ((String)v).Assembly());
         manager.RegisterMessage(this, "concat", v => ((String)v).Concat());
         manager.RegisterMessage(this, "isBin", v => ((String)v).IsBin());
         manager.RegisterMessage(this, "isHex", v => ((String)v).IsHex());
         manager.RegisterMessage(this, "words", v => ((String)v).Words());
         manager.RegisterMessage(this, "field", v => ((String)v).Word());
         manager.RegisterMessage(this, "accent", v => ((String)v).Accent());
         manager.RegisterMessage(this, "camel", v => ((String)v).Camel());
         manager.RegisterMessage(this, "pascal", v => ((String)v).Pascal());
         manager.RegisterMessage(this, "c", v => ((String)v).CCase());
         manager.RegisterMessage(this, "strings", v => ((String)v).Strings());
         manager.RegisterMessage(this, "format", v => ((String)v).Format());
         manager.RegisterMessage(this, "date", v => ((String)v).Date());
         manager.RegisterMessage(this, "int", v => ((String)v).InterpolatedString());
         manager.RegisterMessage(this, "invoke", v => ((String)v).Invoke());
         manager.RegisterMessage(this, "head", v => ((String)v).Head());
         manager.RegisterMessage(this, "tail", v => ((String)v).Tail());
         manager.RegisterMessage(this, "splitMap", v => ((String)v).SplitMap());
         manager.RegisterMessage(this, "abbr", v => ((String)v).Abbreviate());
         manager.RegisterMessage(this, "throw", v => ((String)v).Throw());
         manager.RegisterMessage(this, "file", v => ((String)v).File());
         manager.RegisterMessage(this, "folder", v => ((String)v).Folder());
         manager.RegisterMessage(this, "nav", v => ((String)v).Navigator());
         manager.RegisterMessage(this, "arr", v => ((String)v).ToArray());
         manager.RegisterMessage(this, "apply", v => ((String)v).Apply());
         manager.RegisterMessage(this, "applyWhile", v => ((String)v).ApplyWhile());
         manager.RegisterMessage(this, "regex", v => ((String)v).Regex());
         manager.RegisterMessage(this, "map", v => ((String)v).Map());
         manager.RegisterMessage(this, "skip", v => ((String)v).Skip());
         manager.RegisterMessage(this, "skipWhile", v => ((String)v).SkipWhile());
         manager.RegisterMessage(this, "skipUntil", v => ((String)v).SkipUntil());
         manager.RegisterMessage(this, "take", v => ((String)v).Take());
         manager.RegisterMessage(this, "takeWhile", v => ((String)v).TakeWhile());
         manager.RegisterMessage(this, "takeUntil", v => ((String)v).TakeUntil());
         manager.RegisterMessage(this, "if", v => ((String)v).If());
         manager.RegisterMessage(this, "list", v => ((String)v).ToList());
         manager.RegisterMessage(this, "awk", v => ((String)v).Awk());
         manager.RegisterMessage(this, "foldr", v => ((String)v).FoldR());
         manager.RegisterMessage(this, "foldl", v => ((String)v).FoldL());
         manager.RegisterProperty(this, "item", v => ((String)v).GetItem());
         manager.RegisterMessage(this, "isPunct", v => ((String)v).IsPunctuation());
         manager.RegisterMessage(this, "isMatch", v => ((String)v).IsMatch());
         manager.RegisterMessage(this, "isNotMatch", v => ((String)v).IsNotMatch());
         manager.RegisterMessage(this, "match", v => ((String)v).Match());
         manager.RegisterMessage(this, "matches", v => ((String)v).Matches());
         manager.RegisterMessage(this, "finds", v => ((String)v).Finds());
      }

      public Value Upper() => getText().ToUpper();

      public Value Lower() => getText().ToLower();

      public Value Concat() => getText() + Arguments[0].Text;

      public Value SwapCase()
      {
         var result = new StringBuilder(getText());
         for (var i = 0; i < result.Length; i++)
         {
            var c = result[i];
            result[i] = char.IsUpper(c) ? char.ToLower(c) : char.ToUpper(c);
         }

         return result.ToString();
      }

      public Value IsTitle()
      {
         var str = getText();
         return str.ToTitleCase() == str;
      }

      public Value IsSpace() => getText().IsMatch("^ /s+ $");

      public Value URLEncode() => Uri.EscapeUriString(getText()).Replace("%20", "+");

      public Value URLDecode() => Uri.UnescapeDataString(getText());

      public Value ExpandTabs()
      {
         var countValue = Arguments[0];
         var count = countValue.IsEmpty ? 8 : countValue.Int;
         var replacement = " ".Repeat(count);
         var matcher = new Matcher();
         if (matcher.IsMatch(getText(), "/t", false, true))
            for (var i = 0; i < matcher.MatchCount; i++)
               matcher[i] = replacement;

         return matcher.ToString();
      }

      public Value XML()
      {
         XElement element;
         using (TextReader reader = new StringReader(getText()))
            element = XElement.Load(reader);
         return new XMLElement(element);
      }

      public Value Assign()
      {
         var str = getText();
         var values = State.FieldPattern.Split(str);
         Regions["$0"] = str;
         for (var i = 0; i < values.Length; i++)
            Regions["$" + (i + 1)] = values[i];

         return null;
      }

      public Value FRecs()
      {
         var str = getText();
         var values = State.RecordPattern.Split(str);
         return new FArray(values);
      }

      public Value Get()
      {
         var str = getText();
         var records = State.RecordPattern.Split(str);
         switch (records.Length)
         {
            case 0:
               return "";
            case 1:
               text = "";
               return str;
         }

         var result = records[0];
         text = records.Where((record, i) => i > 0).Listify(State.RecordSeparator.text);
         return result;
      }

      public override Value AlternateValue(string message)
      {
         if (message == "join")
            return null;

         if (IsEmpty)
         {
            if (message.StartsWith("$"))
               return new Array();

            switch (message)
            {
               case "push":
               case "pop":
               case "shift":
               case "unshift":
                  return new Array();
            }
         }

         switch (message)
         {
            case "smap":
            case "for":
               return new Array
               {
                  this
               };
         }

         return Number;
      }

      char[] getTrimChars()
      {
         var value = Arguments[0];
         return value.IsEmpty ? null : Runtime.Expand(value.Text).ToCharArray();
      }

      public Value Strip()
      {
         var str = getText();
         if (str.IsEmpty())
            return "";

         var chars = getTrimChars();
         return chars == null ? str.Trim() : str.Trim(chars);
      }

      public Value LStrip()
      {
         var str = getText();
         if (str.IsEmpty())
            return "";

         var chars = getTrimChars();
         return chars == null ? str.TrimStart() : str.TrimStart(chars);
      }

      public Value RStrip()
      {
         var str = getText();
         if (str.IsEmpty())
            return "";

         var chars = getTrimChars();
         return chars == null ? str.TrimEnd() : str.TrimEnd(chars);
      }

      public Value Truncate()
      {
         var count = Arguments[0].Int;
         var reserved = Arguments[1].Int;
         var ellipses = true;
         if (count < 0)
         {
            ellipses = false;
            count = -count;
         }
         var str = getText();
         if (reserved == 0)
            return str.Truncate(count, ellipses);

         var reverse = str.Reverse();
         var matcher = new Matcher();
         if (matcher.IsMatch(reverse, "/(/s*) /(/w+) /(/s*)") && matcher.MatchCount > reserved + 1)
         {
            var index = reserved;
            matcher[index, 1] = "";
            matcher[index, 2] = "...";
            matcher[index, 3] = "";
            for (var i = reserved + 1; i < matcher.MatchCount && matcher.ToString().Length > count; i++)
               matcher[i] = "";

            var result = matcher.ToString().Reverse();
            if (result.Length <= count)
               return result.Substitute("/s* '...' /s*", "...");
         }

         return str.Truncate(count, ellipses);
      }

      public Value Push()
      {
         var item = Arguments[0].Text;
         text += item;
         return this;
      }

      public Value Pop()
      {
         var str = getText();
         if (str.IsEmpty())
            return "";

         var result = str.Right(1).FlatMap(r => r, () => "");
         text = str.Take(str.Length - 1);
         return result;
      }

      public Value Unshift()
      {
         var item = Arguments[0].Text;
         text = item + getText();
         return this;
      }

      public Value Shift()
      {
         var str = getText();
         if (str.IsEmpty())
            return "";

         var result = str.Substring(0, 1);
         text = str.Substring(1);
         return result;
      }

      public Value Cube() => new Cube(getText());

      public Value SPrint()
      {
         var buffer = getBuffer("sprint");
         buffer.Print(getText());
         return this;
      }

      public Value SPut()
      {
         var buffer = getBuffer("sput");
         buffer.Put(getText());
         return this;
      }

      public Value SWrite()
      {
         var buffer = getBuffer("swrite");
         buffer.Write(getText());
         return this;
      }

      Buffer getBuffer(string messageName)
      {
         var variableName = Arguments[0].Text;
         Assert(variableName.IsNotEmpty(), LOCATION, $"Need a variable name for {messageName}");
         var value = Regions[variableName];
         Buffer buffer;
         if (value.Type != ValueType.Buffer)
         {
            buffer = new Buffer(value.Text);
            Regions[variableName] = buffer;
         }
         else
            buffer = (Buffer)value;
         return buffer;
      }

      public Value XMLify() => getText().XMLify();

      public Value UnXMLify() => getText().UnXMLify();

      public Value XTidy() => getText().Tidy(false);

      public Value ToBase64() => getText().ToBase64(UTF8);

      public Value FromBase64() => getText().FromBase64(UTF8);

      public Value InvertCase()
      {
         var str = getText();
         var result = new StringBuilder();
         foreach (var chr in str)
            result.Append(char.IsUpper(chr) ? char.ToLower(chr) : char.ToUpper(chr));

         return result.ToString();
      }

      static string squeeze(string text, string needle)
      {
         if (needle.IsEmpty())
            needle = " ";
         needle = Runtime.Expand(needle);
         var extra = "";
         if (needle.Contains("'"))
         {
            needle = needle.Replace("'", "");
            extra = " squote";
         }
         if (needle.Contains("\""))
         {
            needle = needle.Replace("\"", "");
            extra += " dquote";
         }
         return needle.Aggregate(text, (current, chr) => current.Substitute($"['{chr}'{extra}]+", chr.ToString()));
      }

      public Value Squeeze()
      {
         var str = getText();
         var needle = Arguments[0].Text;
         return squeeze(str, needle);
      }

      public Value Each()
      {
         var block = Arguments.Executable;
         if (block.CanExecute)
         {
            var valueVar = Arguments.VariableName(0, VAR_VALUE);
            var indexVar = Arguments.VariableName(1, VAR_INDEX);
            var str = getText();
            var chrs = str.ToCharArray().Select(c => c.ToString()).ToArray();
            var slicer = new Slicer(str);
            for (var i = 0; i < chrs.Length; i++)
            {
               Regions.SetLocal(valueVar, chrs[i]);
               Regions.SetLocal(indexVar, i);
               var value = block.Evaluate();
               if (value != null)
                  slicer[i, 1] = value.Text;
            }

            return slicer.ToString();
         }

         return this;
      }

      public Value Visible()
      {
         var str = getText();
         str = str.Replace("\t", "¬");
         str = str.Replace(" ", "•");
         str = str.Replace("\r", "µ");
         str = str.Replace("\n", "¶");
         return str;
      }

      public Value Split()
      {
         var possiblePattern = Arguments[0];
         var str = getText();
         if (possiblePattern is Pattern pattern)
            return new Array(pattern.Split(str));

         if (possiblePattern is Regex possibleRegex)
            return possibleRegex.Split(str);

         if (possiblePattern.IsNumeric())
         {
            var array = new Array();
            var increment = possiblePattern.Int;
            var length = str.Length;
            var i = 0;
            for (; i < length; i += increment)
            {
               var slice = str.Skip(i).Take(increment);
               if (slice.IsNotEmpty())
                  array.Add(slice);
            }

            i -= increment;
            if (i < length)
               array.Add(str.Skip(i));
            return array;
         }

         var regexPattern = possiblePattern.Text.Escape();
         return new Array(str.Split(regexPattern));
      }

      public Value Where()
      {
         var block = Arguments.Executable;
         if (block.CanExecute)
         {
            var valueVar = Arguments.VariableName(0, VAR_VALUE);
            var indexVar = Arguments.VariableName(1, VAR_INDEX);
            var result = new List<string>();
            var index = 0;
            foreach (var record in getRecords())
            {
               Regions.SetLocal(valueVar, record);
               Regions.SetLocal(indexVar, index++);
               if (block.Evaluate().IsTrue)
                  result.Add(record);
            }

            return result.Listify(State.RecordSeparator.Text);
         }

         return "";
      }

      string[] getRecords() => State.RecordPattern.Split(getText());

      public Value First() => getText().Take(1);

      public Value NotFirst() => new Array(getRecords().Where((s, i) => i != 0).ToArray());

      public Value Last() => getText().Skip(getText().Length - 1).Take(1);

      public Value NotLast()
      {
         var records = getRecords();
         var last = records.Length - 1;
         return new Array(records.Where((s, i) => i != last).ToArray());
      }

      public Value Middle()
      {
         var records = getRecords();
         var last = records.Length - 1;
         return new Array(records.Where((s, i) => i != 0 && i != last).ToArray());
      }

      public Value NotMiddle()
      {
         var records = getRecords();
         var last = records.Length - 1;
         return new Array(new[]
         {
            records[0],
            records[last]
         });
      }

      public Value AddField()
      {
         var str = getText();
         var arg = Arguments[0].Text;
         if (str.IsNotEmpty())
            str += State.FieldSeparator + arg;
         else
            str = arg;
         return str;
      }

      public Value AddRecord()
      {
         var str = getText();
         var arg = Arguments[0].Text;
         if (str.IsNotEmpty())
            str += State.RecordSeparator.Text + arg;
         else
            str = arg;
         return str;
      }

      public Value Succ()
      {
         var value = getText();
         if (value.IsEmpty())
            return 1;

         return text.Succ();
      }

      public Value Pred()
      {
         var value = getText();
         if (value.IsEmpty())
            return -1;

         return text.Pred();
      }

      public Value Expand() => Runtime.Expand(getText());

      public Value Justify()
      {
         var length = Arguments[0].Int;
         var oldText = getText();
         var textLength = oldText.Length;
         if (length <= textLength)
            return oldText.Substring(0, length);

         var slicer = new Matcher();
         if (!slicer.IsMatch(oldText, "/s+"))
            return oldText.PadRight(length);

         var difference = length - textLength;
         var matchCount = slicer.MatchCount;
         var matchIndex = 0;
         for (var i = 0; i < difference; i++)
         {
            slicer[matchIndex] += " ";
            if (++matchIndex < matchCount)
               continue;

            slicer.Evaluate(slicer.ToString(), "/s+");
            matchIndex = 0;
         }

         return slicer.ToString();
      }

      public Value FindFIeld()
      {
         var needle = Arguments[0].Text;
         var fields = State.FieldPattern.Split(getText());
         for (var i = 0; i < fields.Length; i++)
         {
            var field = fields[i];
            if (needle == field)
               return i;
         }

         return -1;
      }

      static string getPadding(string text, string padding = " ") => text.IsEmpty() ? padding : text.Substring(0, 1);

      static char getPaddingAsChar(string text, string padding = " ") => getPadding(text, padding)[0];

      public Value CompareTo()
      {
         var comparisand1 = getText();
         var comparisand2 = Arguments[0].Text;
         var padding = getPaddingAsChar(Arguments[1].Text);
         var length = Math.Max(comparisand1.Length, comparisand2.Length);
         comparisand1 = comparisand1.PadRight(length, padding);
         comparisand2 = comparisand2.PadRight(length, padding);
         for (var i = 0; i < length; i++)
            if (comparisand1[i] != comparisand2[i])
               return i;

         return 0;
      }

      public Value Compare()
      {
         var comparisand = Arguments[0].Text;
         return string.Compare(getText(), comparisand, Ordinal);
      }

      public Value Command()
      {
         FileName file = @"c:\windows\system32\cmd.exe";
         var result = file.Execute("/C \"" + getText() + "\"");
         return result;
      }

      public Value Reverse() => getText().Reverse();

      Value indexOf(string needle, int start)
      {
         var str = getText();
         if (start >= str.Length)
            return None.NoneValue;

         var index = str.IndexOf(needle, start, Ordinal);
         return index == -1 ? (Value)None.NoneValue : new Some(index);
      }

      Value indexBackOf(string str, string needle, int start)
      {
         if (start >= str.Length)
            return None.NoneValue;

         var index = str.LastIndexOf(needle, start, Ordinal);
         return index == -1 ? (Value)None.NoneValue : new Some(index);
      }

      public Value Find()
      {
         var needle = Arguments[0].Text;
         var start = Arguments[1].Int;
         return indexOf(needle, start);
      }

      public Value FindBack()
      {
         var str = getText();
         var needle = Arguments[0].Text;
         var startText = Arguments[1].Text;
         var start = startText.IsEmpty() ? str.Length - 1 : startText.ToInt();
         return indexBackOf(str, needle, start);
      }

      public Value Indexes()
      {
         var needleValue = Arguments[0];
         if (needleValue.IsEmpty)
            return new Array();

         var needle = needleValue.Text;
         var str = getText();
         var array = new Array();
         var index = str.IndexOf(needle, 0, Ordinal);
         var needleLength = needle.Length;
         while (index > -1)
         {
            array.Add(index);
            index = str.IndexOf(needle, index + needleLength, Ordinal);
         }

         return array;
      }

      public Value LastIndexOf()
      {
         var needle = Arguments[0].Text;
         var str = getText();
         return str.LastIndexOf(needle, str.Length - 1, Ordinal);
      }

      public Value Space()
      {
         var count = Arguments[0].Int;
         var spaces = " ".Repeat(count);
         return State.FieldPattern.Split(getText()).Listify(spaces);
      }

      public Value DeleteField()
      {
         var index = Arguments[0].Int;
         var count = Arguments.DefaultTo(1, 1).Int;
         var fields = State.FieldPattern.Split(getText());
         if (index < -1)
            index = WrapIndex(index, fields.Length, true);
         var result = new List<string>();
         for (var i = 0; i < index; i++)
            result.Add(fields[i]);
         for (var i = index + count; i < fields.Length; i++)
            result.Add(fields[i]);

         return result.Listify(State.FieldSeparator.text);
      }

      public Value Delete()
      {
         var index = Arguments[0].Int;
         var lengthValue = Arguments[1];
         Slicer slicer = getText();
         var length = lengthValue.IsEmpty ? slicer.Length - index : lengthValue.Int;
         slicer[index, length] = "";
         return slicer.ToString();
      }

      public Value Overlay()
      {
         var oldText = getText();
         var replacement = Arguments[0].Text;
         var padding = getPaddingAsChar(Arguments[1].Text, "~");
         var length = Math.Max(oldText.Length, replacement.Length);
         oldText = oldText.PadRight(length, padding);
         replacement = replacement.PadRight(length, padding);
         var result = new StringBuilder(oldText);
         for (var i = 0; i < length; i++)
            if (replacement[i] != '~')
               result[i] = replacement[i];

         return result.Replace('~', ' ').ToString();
      }

      public Value Insert()
      {
         var newText = Arguments[0].Text;
         var index = Arguments[1].Int;
         var oldText = getText();
         if (index < 0)
            index = WrapIndex(index, oldText.Length, false);
         if (index > oldText.Length)
         {
            var padding = getPaddingAsChar(Arguments[2].Text);
            oldText = oldText.PadRight(index, padding);
         }
         return oldText.Insert(index, newText);
      }

      public Value Field()
      {
         var index = Arguments[0].Int;
         var fields = State.FieldPattern.Split(getText());
         if (index < -1)
            index = WrapIndex(index, fields.Length, true);
         return fields.Of(index, "");
      }

      public Value Left()
      {
         var amount = Arguments[0].Int;
         var str = getText();
         return str.Left(amount).FlatMap(s => s.PadRight(amount), () => str);
      }

      public Value Right()
      {
         var amount = Arguments[0].Int;
         var str = getText();
         return str.Right(amount).FlatMap(s => s.PadLeft(amount), () => str);
      }

      public Value Replace()
      {
         var argument0 = Arguments[0];
         if (argument0 is Regex regex)
         {
            var arguments = new Arguments();
            arguments.AddArgument(getText());
            arguments.AddArgument(Arguments[1]);
            regex.Arguments = arguments;
            return regex.Replace();
         }

         var needle = argument0.Text;
         if (needle.IsEmpty())
            return this;

         var replacement = Arguments[1].Text;
         return getText().Replace(needle, replacement);
      }

      public Value ReplaceAll()
      {
         if (Arguments[0] is Regex regex)
         {
            var arguments = new Arguments();
            arguments.AddArgument(getText());
            arguments.AddArgument(Arguments[1]);
            regex.Arguments = arguments;
            return regex.ReplaceAll();
         }

         return Replace();
      }

      public Value Substring()
      {
         var index = Arguments[0].Int;
         var length = Arguments[1].Int;
         var replacement = Arguments[2].Text;
         var s = getText();
         var skipped = s.Skip(index);
         if (replacement.IsEmpty())
            return Arguments[1].IsEmpty ? skipped : skipped.Take(length);

         var prefix = s.Take(index) + replacement;
         return Arguments[1].IsEmpty ? prefix : prefix + s.Skip(index + length);
      }

      public Value Evaluate()
      {
         if (GetBlock(getText(), 0, true).If(out var block, out var _))
         {
            Regions.Push("evaluate()");
            MessagingState.TemplateMode = true;
            var result = block.Evaluate();
            MessagingState.TemplateMode = false;
            Regions.Pop("evaluate()");
            return result;
         }

         return NilValue;
      }

      string charMap(string from, string to, bool ignore)
      {
         from = Runtime.Expand(from);
         to = Runtime.Expand(to);
         if (!ignore && to.Length == 1 && from.Length > 1)
            to = to.Repeat(from.Length);
         var mapping = new AutoHash<string, string>(k => k);

         var length = Math.Min(to.Length, from.Length);
         for (var i = 0; i < length; i++)
            mapping[from[i].ToString()] = to[i].ToString();

         if (ignore)
         {
            var result = new StringBuilder();
            for (var i = 0; i < getText().Length; i++)
            {
               var letter = getText().Substring(i, 1);
               var mapped = mapping[letter];
               if (!(from.Has(letter) && !to.Has(mapped)))
                  result.Append(mapped);
            }

            return result.ToString();
         }

         var slicer = new Slicer(getText());
         for (var i = 0; i < getText().Length; i++)
            slicer[i] = mapping[slicer[i]];

         return slicer.ToString();
      }

      public Value CharMap(bool ignore)
      {
         var from = Arguments[0].Text;
         var to = Arguments[1].Text;

         return charMap(from, to, ignore);
      }

      public Value CharMapC()
      {
         var from = Runtime.Expand(Arguments[0].Text);
         var to = Runtime.Expand(Arguments[1].Text);
         var result = new StringBuilder();
         if (to.IsEmpty())
            foreach (var c in getText().Where(c => from.IndexOf(c) > -1))
               result.Append(c);
         else
         {
            var replacement = to[0];
            foreach (var c in getText())
               result.Append(from.IndexOf(c) > -1 ? c : replacement);
         }

         return result.ToString();
      }

      public Value CharMapS()
      {
         var from = Runtime.Expand(Arguments[0].Text);
         var to = Runtime.Expand(Arguments[1].Text);
         return squeeze(charMap(from, to, false), to);
      }

      public Value Transpose()
      {
         var source = Runtime.Expand(getText());
         var map = new AutoHash<char, char>(k => k);
         var strMap = Runtime.Expand(Arguments[0].Text);
         var minLength = Math.Min(source.Length, strMap.Length);
         for (var i = 0; i < minLength; i++)
            map[strMap[i]] = source[i];

         var result = new StringBuilder();
         var target = Arguments[1].Text;
         foreach (var c in target)
            result.Append(map[c]);

         return result.ToString();
      }

      public Value Title() => getText().ToTitleCase();

      public Value MatchCase()
      {
         var target = getText();
         var subject = Arguments[0].Text;
         if (subject.IsEmpty())
            return target;

         var targetMatcher = new Matcher();
         if (!targetMatcher.IsMatch(target, "/w+"))
            return target;

         var subjectMatcher = new Matcher();
         if (!subjectMatcher.IsMatch(subject, "/w+"))
            return target;

         var length = Math.Min(targetMatcher.MatchCount, subjectMatcher.MatchCount);
         var lastSubjectWord = "";
         for (var i = 0; i < length; i++)
         {
            lastSubjectWord = subjectMatcher[i];
            targetMatcher[i] = convertWord(lastSubjectWord, targetMatcher[i]);
         }
         for (var i = length; i < targetMatcher.MatchCount; i++)
            targetMatcher[i] = convertWord(lastSubjectWord, targetMatcher[i]);

         return targetMatcher.ToString();
      }

      static bool isAllCaps(string subject) => subject.ToUpper() == subject;

      static bool isAllLower(string subject) => subject.ToLower() == subject;

      static string convertWord(string subject, string target)
      {
         if (isAllCaps(subject))
            return target.ToUpper();
         if (isAllLower(subject))
            return target.ToLower();

         var length = Math.Min(subject.Length, target.Length);
         var result = new StringBuilder(target);
         var isUpper = false;
         for (var i = 0; i < length; i++)
         {
            var c = subject[i];
            isUpper = char.IsUpper(c);
            result[i] = isUpper ? char.ToUpper(result[i]) : char.ToLower(result[i]);
         }

         if (isUpper)
            for (var i = length; i < target.Length; i++)
               result[i] = char.ToUpper(result[i]);
         else
            for (var i = length; i < target.Length; i++)
               result[i] = char.ToLower(result[i]);

         return result.ToString();
      }

      public Value SQuote() => $"'{getText().Replace("'", "`'")}'";

      public Value DQuote() => $"\"{getText().Replace("\"", "`\"")}\"";

      public Value Max()
      {
         var max = char.MinValue;
         foreach (var c in getText().Where(c => c > max))
            max = c;

         return max.ToString();
      }

      public Value Min()
      {
         var min = char.MaxValue;
         foreach (var c in getText().Where(c => c < min))
            min = c;

         return min.ToString();
      }

      public Value IsAlpha() => getText().IsMatch(REGEX_ALPHA);

      public Value IsAlphaNum() => getText().IsMatch(REGEX_ALPHANUM);

      public Value IsUpper() => getText().Select(c => c.ToString()).All(s => s.IsMatch("['A-Z']"));

      public Value IsLower() => getText().Select(c => c.ToString()).All(s => s.IsMatch("['a-z']"));

      public Value IsDigit() => getText().IsMatch("^ /d+ $");

      public Value StartsWith() => getText().StartsWith(Arguments[0].Text);

      public Value EndsWith() => getText().EndsWith(Arguments[0].Text);

      public Value Seek() => getText().IndexOf(Arguments[0].Text, Ordinal);

      public Value SeekLast() => getText().LastIndexOf(Arguments[0].Text, Ordinal);

      public Value Count()
      {
         var matcher = new Matcher();
         return matcher.IsMatch(getText(), Arguments[0].Text.Quotify(), false, true) ? matcher.MatchCount : 0;
      }

      public Value PadLeft()
      {
         var width = Arguments[0].Int;
         var padding = Arguments[1].Text;
         if (padding.IsEmpty())
            padding = " ";
         return PadLeft(getText(), width, padding);
      }

      public Value PadRight()
      {
         var width = Arguments[0].Int;
         var padding = Arguments[1].Text;
         if (padding.IsEmpty())
            padding = " ";
         return PadRight(getText(), width, padding);
      }

      public Value Center()
      {
         var width = Arguments[0].Int;
         var padding = Arguments[1].Text;
         if (padding.IsEmpty())
            padding = " ";
         return PadCenter(getText(), width, padding);
      }

      public Value Ord()
      {
         var str = getText();
         if (str.Length == 0)
            return "";

         var ord = (int)str[0];
         return ord;
      }

      public Value Has() => getText().IndexOf(Arguments[0].Text, Ordinal) > -1;

      public Value Comparison() => string.Compare(getText(), Arguments[0].Text, Ordinal);

      public Value Capital()
      {
         var str = getText();
         if (str.IsEmpty())
            return "";

         return char.ToUpper(str.Take(1)[0]) + str.Skip(1).ToLower();
      }

      public Value To(string stop)
      {
         var array = new Array();

         if (stop.IsEmpty())
            return array;

         var start = getText();
         if (start.IsEmpty())
            return array;

         var length = stop.Length;
         if (string.Compare(start, stop, Ordinal) < 0)
            while (string.Compare(start, stop, Ordinal) <= 0 && start.Length == length)
            {
               array.Add(start);
               start = start.Succ();
            }
         else
            while (string.Compare(start, stop, Ordinal) >= 0 && start.Length == length)
            {
               array.Add(start);
               start = start.Pred();
            }

         return array;
      }

      public Value To() => To(Arguments[0].Text);

      public Value Over()
      {
         var start = (int)getText()[0];
         var stop = Arguments[0].Int;
         var increment = Arguments.DefaultTo(1, 1).Int;
         stop = start + stop - 1;
         var array = new Array();
         if (start <= stop)
            for (var i = start; i <= stop; i += increment)
               array.Add(((char)i).ToString());
         else
            for (var i = start; i >= stop; i -= increment)
               array.Add(((char)i).ToString());

         return array;
      }

      public Value Fields() => new Array(State.FieldPattern.Split(getText()));

      public Value SetRecordSeparator()
      {
         var executable = Arguments.Executable;
         if (executable.CanExecute)
         {
            var oldSeparator = State.RecordSeparator;
            State.RecordSeparator = this;
            var value = executable.Evaluate();
            State.RecordSeparator = oldSeparator;
            return value;
         }

         State.RecordSeparator = this;
         return this;
      }

      public Value SetFieldSeparator()
      {
         var executable = Arguments.Executable;
         if (executable.CanExecute)
         {
            var oldSeparator = State.FieldSeparator;
            State.FieldSeparator = this;
            var value = executable.Evaluate();
            State.FieldSeparator = oldSeparator;
            return value;
         }

         State.FieldSeparator = this;
         return this;
      }

      public Value Chars()
      {
         var array = new Array();
         var str = text;
         for (var i = 0; i < str.Length; i++)
            array.Add(str.Substring(i, 1));

         return array;
      }

      public Value Recs()
      {
         Value value = Arguments[0].Text;
         string[] records;
         if (value.IsEmpty)
         {
            records = State.RecordPattern.Split(getText());
            return new Array(records);
         }

         if (value is Pattern pattern)
         {
            records = pattern.Split(getText());
            return new Array(records);
         }

         var strPattern = value.Text.Escape();
         records = getText().Split(strPattern);
         return new Array(records);
      }

      public Value In() => getText().IndexOf(Arguments[0].Text, Ordinal) > -1;

      public Value NotIn() => !In().IsTrue;

      public Value Same() => getText().Same(Arguments[0].Text);

      public Value NotSame() => !getText().Same(Arguments[0].Text);

      public Value Repeat() => getText().Repeat(Arguments[0].Int);

      public override string ToString() => getText().DefaultTo("").Replace("\r", "`r").Replace("\n", "`n")
         .Replace("\t", "`t").Quotify(@"`""");

      public Value Repeat(int count) => getText().Repeat(count);

      public override ValueType Type => ValueType.String;

      public override bool IsTrue => getText().IsNotEmpty();

      public override Value Clone() => new String(text.Copy());

      public Value Ref()
      {
         var str = text;
         Reject(str.IsEmpty(), LOCATION, "Variable can't be created from an empty string");
         return new Variable(str);
      }

      public static Value Ask()
      {
         RejectNull(State.Asker == null, LOCATION, "Asker not set");
         return State.Asker();
      }

      public Value Lines() => new Lines(getText());

      public Value FLines() => new FileLines(getText());

      public Value Append()
      {
         var buffer = new Buffer(getText());
         buffer.Print(Arguments[0].Text);
         return buffer;
      }

      public Value Symbol() => new Symbol(getText());

      public Value Unstring()
      {
         var destringifier = new Destringifier(getText())
         {
            SingleQuote = '\'',
            DoubleQuote = '"',
            Escape = '`'
         };
         var destringified = destringifier.Parse();
         var array = new Array(destringifier.Strings);
         if (Arguments.Executable.CanExecute)
         {
            var block = Arguments.Executable;
            block.AutoRegister = false;
            State.RegisterBlock(block);
            var variable = Arguments.VariableName(0, VAR_VALUE);
            var arrayVar = Arguments.VariableName(1, "$arr");
            Regions.SetLocal(variable, destringified);
            Regions.SetLocal(arrayVar, array);
            var result = block.Evaluate();
            result = State.UseReturnValue(result);
            State.UnregisterBlock();
            return destringifier.Restring(result.Text, true);
         }

         return new Array { destringified, array };
      }

      public Value Assembly() => new AssemblyValue(getText());

      public Value IsBin() => getText().IsMatch("^ ['01_,']+ $");

      public Value IsHex() => getText().IsMatch("^ ['0-9a-fA-F_,']+ $");

      public Value Words()
      {
         var str = getText();
         var matcher = new Matcher();
         string before;
         if (matcher.IsMatch(str, "^ /(/s+) /(.+) $"))
         {
            before = matcher[0, 1];
            str = matcher[0, 2];
         }
         else
            before = "";
         var array = new Array();
         matcher.Evaluate(str, "/([squote 'a-zA-Z0-9_-']+) /(-[squote 'a-zA-Z0-9_-']+)?");
         var lastWord = none<Word>();
         for (var i = 0; i < matcher.MatchCount; i++)
         {
            var word = matcher[i, 1];
            var after = matcher[i, 2];
            var index = matcher.GetMatch(i).Index;
            var value = new Word(word, index, i, before, after);
            if (lastWord.IsNone)
               value.SetPrevious(null);
            else
            {
               value.SetPrevious(lastWord.Value);
               lastWord.Value.SetNext(value);
            }
            array.Add(value);
            lastWord = value.Some();
            before = "";
         }

         return array;
      }

      public Value Word()
      {
         var index = Arguments[0].Int;
         var words = (Array)Fields();
         return words[index];
      }

      public Value Accent()
      {
         var str = getText();
         for (var i = 0; i < STRING_ACCENTS.Length; i += 3)
         {
            var start = 0;
            var index = 0;
            Slicer slicer = str;
            var search = STRING_ACCENTS.Substring(i, 2);
            var replacement = STRING_ACCENTS.Substring(i + 2, 1);
            while (index != -1 && start < slicer.Length)
            {
               index = slicer.ToString().IndexOf(search, start, Ordinal);
               slicer.Reset();
               if (index <= -1)
                  break;

               slicer[index, 2] = replacement;
               start = index + 3;
            }

            str = slicer.ToString();
         }

         return str;
      }

      public Value Camel()
      {
         var matcher = new Matcher();
         var input = getText();
         if (matcher.IsMatch(input, "/w+"))
         {
            for (var i = 0; i < matcher.MatchCount; i++)
               matcher[i] = matcher[i].SnakeToCamelCase(false);

            var result = matcher.ToString();
            if (matcher.IsMatch(result, "-/w+"))
            {
               for (var i = 0; i < matcher.MatchCount; i++)
                  matcher[i] = "";

               result = matcher.ToString();
            }

            if (matcher.IsMatch(result, "^ ['A-Z']"))
            {
               matcher[0] = matcher[0].ToLower();
               result = matcher.ToString();
            }
            return result;
         }

         return input;
      }

      public Value Pascal()
      {
         var matcher = new Matcher();
         var input = getText();
         if (matcher.IsMatch(input, "/w+"))
         {
            for (var i = 0; i < matcher.MatchCount; i++)
               matcher[i] = matcher[i].SnakeToCamelCase(true);

            var result = matcher.ToString();
            if (matcher.IsMatch(result, "-/w+"))
            {
               for (var i = 0; i < matcher.MatchCount; i++)
                  matcher[i] = "";

               result = matcher.ToString();
            }

            if (matcher.IsMatch(result, @"^ ['a-z']"))
            {
               matcher[0] = matcher[0].ToUpper();
               result = matcher.ToString();
            }
            return result;
         }

         return input;
      }

      public Value CCase() => getText().CamelToSnakeCase();

      public Value Strings()
      {
         var includeQuotes = Arguments[0].IsTrue;
         var str = getText();
         var destringifier = new Destringifier(str);
         var parsed = destringifier.Parse();
         var split = State.FieldPattern.Split(parsed);
         for (var i = 0; i < split.Length; i++)
            split[i] = destringifier.Restring(split[i], includeQuotes);

         return new Array(split);
      }

      public Value IsVowel() => getText().IsMatch("^ ['aeiou']+ $");

      public Value IsConsonant() => getText().IsMatch("^ -['aeiou']+ $");

      public Value IsPunctuation() => getText().IsMatch("^ [punct]+ $");

      public virtual Value Format()
      {
         var array = Arguments.AsArray();
         if (array == null)
            return this;

         var matcher = new Matcher();
         if (matcher.IsMatch(getText(), @"'\' /(/d+)"))
         {
            for (var i = 0; i < matcher.MatchCount; i++)
               matcher[i] = array[matcher[i, 1].ToInt()].Text;

            return matcher.ToString();
         }

         return this;
      }

      public Array ToArray() => new Array(toArray());

      string[] toArray() => getText().Select(c => c.ToString()).ToArray();

      public Value Date() => new Date(this);

      public Value InterpolatedString()
      {
         try
         {
            Parser.Coloring = false;
            var parser = new InterpolatedStringParser2();
            var str = "$'" + getText().Replace("'", "`'") + "'";
            return parser.Scan(str, 0) ? parser.Result.Value : new InterpolatedString("", new List<Block>());
         }
         finally
         {
            Parser.Coloring = true;
         }
      }

      public virtual Value Invoke() => SendMessage(InterpolatedString(), "invoke", Arguments);

      public Value IsIn()
      {
         var haystack = Arguments[0].Text;
         var ignoreCase = Arguments[1].IsTrue;
         return haystack.Has(getText(), ignoreCase);
      }

      public Value Head()
      {
         var str = getText();
         return str.Take(str.Length - 1);
      }

      public Value Tail() => getText().Skip(1);

      public Value SplitMap()
      {
         if (Arguments[0] is Pattern pattern)
         {
            var records = pattern.Split(getText());
            return new Array(records);
         }

         return this;
      }

      public Value Abbreviate()
      {
         var size = Arguments[0].Int;
         if (size <= 0)
            return "";

         var matcher = new Matcher();
         var str = getText();
         if (matcher.IsMatch(str, "-/s+ $"))
         {
            var word = matcher[0];
            var wordLength = word.Length;
            var adjustedSize = size - 5 - wordLength;
            return str.Substring(0, adjustedSize) + " ... " + word;
         }

         return this;
      }

      public Value Throw() => throw new ApplicationException(getText());

      public Value File() => new File(getText());

      public Value Folder() => new Folder(getText());

      public Value Navigator() => new StringNavigator(getText());

      public override Value SourceArray => ToArray();

      public Value Index()
      {
         var str = getText();
         if (str.IsEmpty())
         {
            var array = new Array();
            return SendMessage(array, "index", Arguments);
         }

         return SendMessage(AlternateValue("index"), "index", Arguments);
      }

      public override int GetHashCode() => getText().GetHashCode();

      public virtual Value Apply() => Arguments.ApplyValue is Pattern p ? p.Replace(getText()) : this;

      public Value ApplyWhile() => Arguments.ApplyValue is Pattern p ? p.ReplaceAll(getText()) : this;

      public Value Regex() => new Regex(getText(), false, false, false);

      public Value Map()
      {
         using (var assistant = new ParameterAssistant(Arguments))
         {
            var block = assistant.Block();
            if (block == null)
               return this;

            assistant.LoopParameters();

            var builder = new StringBuilder();

            var str = getText();
            for (var i = 0; i < str.Length; i++)
            {
               var ch = str[i];
               var s = ch.ToString();
               assistant.SetLoopParameters(s, i);
               var value = block.Evaluate();
               var signal = Signal();
               if (signal == Breaking)
                  return builder.ToString();

               switch (signal)
               {
                  case Continuing:
                     continue;
                  case ReturningNull:
                     return null;
               }

               if (value.IsNil)
                  continue;

               foreach (var newChar in value.Text)
                  builder.Append(newChar);
            }

            return builder.ToString();
         }
      }

      public Value Skip() => getText().Skip(Arguments[0].Int);

      public Value SkipWhile()
      {
         using (var assistant = new ParameterAssistant(Arguments))
         {
            var block = assistant.Block();
            if (block == null)
               return this;

            assistant.LoopParameters();

            var str = getText();
            for (var i = 0; i < str.Length; i++)
            {
               var ch = str[i];
               var s = ch.ToString();
               assistant.SetLoopParameters(s, i);
               var value = block.Evaluate();
               var signal = Signal();
               if (signal == Breaking)
                  return "";

               switch (signal)
               {
                  case Continuing:
                     continue;
                  case ReturningNull:
                     return null;
               }

               if (!value.IsTrue)
                  return str.Substring(i);
            }

            return getText();
         }
      }

      public Value SkipUntil()
      {
         using (var assistant = new ParameterAssistant(Arguments))
         {
            var block = assistant.Block();
            if (block == null)
               return this;

            assistant.LoopParameters();

            var str = getText();
            for (var i = 0; i < str.Length; i++)
            {
               var ch = str[i];
               var s = ch.ToString();
               assistant.SetLoopParameters(s, i);
               var value = block.Evaluate();
               var signal = Signal();
               if (signal == Breaking)
                  return "";

               switch (signal)
               {
                  case Continuing:
                     continue;
                  case ReturningNull:
                     return null;
               }

               if (value.IsTrue)
                  return str.Substring(i);
            }

            return getText();
         }
      }

      public Value Take() => getText().Take(Arguments[0].Int);

      public Value TakeWhile()
      {
         using (var assistant = new ParameterAssistant(Arguments))
         {
            var block = assistant.Block();
            if (block == null)
               return this;

            assistant.LoopParameters();

            var str = getText();
            for (var i = 0; i < str.Length; i++)
            {
               var ch = str[i];
               var s = ch.ToString();
               assistant.SetLoopParameters(s, i);
               var value = block.Evaluate();
               var signal = Signal();
               if (signal == Breaking)
                  return "";

               switch (signal)
               {
                  case Continuing:
                     continue;
                  case ReturningNull:
                     return null;
               }

               if (!value.IsTrue)
                  return str.Substring(0, i);
            }

            return getText();
         }
      }

      public Value TakeUntil()
      {
         using (var assistant = new ParameterAssistant(Arguments))
         {
            var block = assistant.Block();
            if (block == null)
               return this;

            assistant.LoopParameters();

            var str = getText();
            for (var i = 0; i < str.Length; i++)
            {
               var ch = str[i];
               var s = ch.ToString();
               assistant.SetLoopParameters(s, i);
               var value = block.Evaluate();
               var signal = Signal();
               if (signal == Breaking)
                  return "";

               switch (signal)
               {
                  case Continuing:
                     continue;
                  case ReturningNull:
                     return null;
               }

               if (value.IsTrue)
                  return str.Substring(0, i);
            }

            return getText();
         }
      }

      public Value If()
      {
         using (var assistant = new ParameterAssistant(Arguments))
         {
            var block = assistant.Block();
            if (block == null)
               return this;

            assistant.LoopParameters();

            var builder = new StringBuilder();

            var str = getText();
            for (var i = 0; i < str.Length; i++)
            {
               var ch = str[i];
               var s = ch.ToString();
               assistant.SetLoopParameters(s, i);
               var value = block.Evaluate();
               var signal = Signal();
               if (signal == Breaking)
                  return builder.ToString();

               switch (signal)
               {
                  case Continuing:
                     continue;
                  case ReturningNull:
                     return null;
               }

               if (value.IsTrue)
                  builder.Append(ch);
            }

            return builder.ToString();
         }
      }

      public Value ToList() => List.FromArray(new Array(getText().ToCharArray().Select(c => c.ToString()).ToArray()));

      public Value Awk()
      {
         var str = getText();
         var fields = State.FieldPattern.Split(str);
         var array = new Array { str };
         foreach (var field in fields)
            array.Add(field);

         return array;
      }

      public Value FoldL()
      {
         var str = getText();

         using (var assistant = new ParameterAssistant(Arguments))
         {
            var block = assistant.Block();
            if (block == null || str.Length == 0)
               return assistant.NilOrClosure;
            if (str.Length == 1)
               return str[0];

            assistant.TwoValueParameters();

            var values = toArray();

            Value initialValue;
            Value secondValue;
            int start;
            var initialFromArguments = Arguments[0];
            if (initialFromArguments.IsEmpty)
            {
               initialValue = values[0];
               secondValue = values[1];
               start = 2;
            }
            else
            {
               initialValue = initialFromArguments;
               secondValue = values[0];
               start = 1;
            }
            assistant.SetParameterValues(initialValue, secondValue);
            var value = block.Evaluate();
            var signal = Signal();
            if (signal == Breaking)
               return value;

            switch (signal)
            {
               case ReturningNull:
                  return null;
               case Continuing:
                  return value;
            }

            RejectNull(value, LOCATION, "Scalar block must return a value");

            for (var i = start; i < values.Length; i++)
            {
               assistant.SetParameterValues(value, values[i]);
               value = block.Evaluate();
               signal = Signal();
               if (signal == Breaking)
                  break;

               switch (signal)
               {
                  case ReturningNull:
                     return null;
                  case Continuing:
                     continue;
               }

               RejectNull(value, LOCATION, "Scalar block must return a value");
            }

            return value;
         }
      }

      public Value FoldR()
      {
         var str = getText();

         using (var assistant = new ParameterAssistant(Arguments))
         {
            var block = assistant.Block();
            if (block == null || str.Length == 0)
               return assistant.NilOrClosure;
            if (str.Length == 1)
               return str[0];

            assistant.TwoValueParameters();

            var values = toArray();

            Value initialValue;
            Value secondValue;
            int start;
            var init = values.Length - 1;
            var initialFromArguments = Arguments[0];
            if (initialFromArguments.IsEmpty)
            {
               initialValue = values[init];
               secondValue = values[init - 1];
               start = init - 2;
            }
            else
            {
               initialValue = initialFromArguments;
               secondValue = values[init];
               start = init - 1;
            }
            assistant.SetParameterValues(secondValue, initialValue);
            var value = block.Evaluate();
            var signal = Signal();
            if (signal == Breaking)
               return value;

            switch (signal)
            {
               case ReturningNull:
                  return null;
               case Continuing:
                  return value;
            }

            RejectNull(value, LOCATION, "Scalar block must return a value");

            for (var i = start; i > -1; i--)
            {
               assistant.SetParameterValues(values[i], value);
               value = block.Evaluate();
               signal = Signal();
               if (signal == Breaking)
                  break;

               switch (signal)
               {
                  case ReturningNull:
                     return null;
                  case Continuing:
                     continue;
               }

               RejectNull(value, LOCATION, "Scalar block must return a value");
            }

            return value;
         }
      }

      public INSGenerator GetGenerator() => new NSGenerator(this);

      public Value Next(int index)
      {
         var str = getText();
         return index < str.Length ? (Value)str.Substring(index, 1) : NilValue;
      }

      public bool IsGeneratorAvailable => true;

      static int[] getSliceArguments(Value[] values, int length)
      {
         var list = new List<int>();
         foreach (var value in values)
         {
            if (value.Int >= length)
               break;

            var generator = value.PossibleIndexGenerator();
            if (generator.IsSome)
            {
               var iterator = new NSIteratorByLength(generator.Value, length);
               list.AddRange(iterator.Select(v => v.Int));
            }
            else
               list.Add(value.Int);
         }

         return list.ToArray();
      }

      public Value GetItem()
      {
         var argumentsValues = Arguments.Values;
         if (argumentsValues.Length > 0 && argumentsValues[0] is Regex regex)
         {
            var index = 0;
            if (argumentsValues.Length > 1)
               index = argumentsValues[1].Int;
            return regex.Slice(getText(), index);
         }

         using (var popper = new RegionPopper(new Region(), "get-item"))
         {
            popper.Push();
            var str = getText();
            Regions.Current.SetParameter("$", str.Length);
            var indexes = getSliceArguments(argumentsValues, str.Length);
            switch (indexes.Length)
            {
               case 0:
                  return "";
               case 1:
                  return getText().Skip(WrapIndex(indexes[0], str.Length, true)).Take(1);
            }

            var builder = new StringBuilder();
            foreach (var chr in indexes.Select(i => WrapIndex(i, str.Length, true)).Select(i => str.Skip(i).Take(1)))
               builder.Append(chr);

            return builder.ToString();
         }
      }

      public override IMaybe<INSGenerator> PossibleIndexGenerator() => none<INSGenerator>();

      public Value IsMatch()
      {
         switch (Arguments[0])
         {
            case Pattern pattern:
               return SendMessage(pattern, "apply", this);
            case Regex regex:
               return SendMessage(regex, "match", this);
            default:
               return false;
         }
      }

      public Value IsNotMatch()
      {
         switch (Arguments[0])
         {
            case Pattern pattern:
               return SendMessage(pattern, "applyNot", this);
            case Regex regex:
               return SendMessage(regex, "match", this) is None;
            default:
               return true;
         }
      }

      public Value Match() => Arguments[0] is Regex regex ? regex.Match(getText()) : this;

      public Value Matches() => Arguments[0] is Regex regex ? regex.Matches(getText()) : this;

      public Value Finds()
      {
         var needle = Arguments[0].Text;
         var str = getText();
         var index = 0;

         return new NSGeneratorSource(this, i =>
         {
            if (i == 0)
               index = 0;
            var result = str.IndexOf(needle, index, Ordinal);
            if (result == -1)
               return NilValue;

            index = result + needle.Length;
            return result;
         });
      }
   }
}