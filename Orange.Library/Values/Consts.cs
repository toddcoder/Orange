using Orange.Library.Managers;
using static System.Math;
using static Orange.Library.CodeBuilder;
using static Orange.Library.Runtime;

namespace Orange.Library.Values
{
   public class Consts : Value
   {
      public override int Compare(Value value) => value is Consts ? 0 : -1;

      public override string Text { get; set; } = "";

      public override double Number { get; set; }

      public override ValueType Type => ValueType.Consts;

      public override bool IsTrue => false;

      public override Value Clone() => this;

      protected override void registerMessages(MessageManager manager)
      {
         manager.RegisterMessage(this, "tab", v => Tab);
         manager.RegisterMessage(this, "crlf", v => CRLF);
         manager.RegisterMessage(this, "cr", v => CR);
         manager.RegisterMessage(this, "lf", v => LF);
         manager.RegisterMessage(this, "letters", v => Letters);
         manager.RegisterMessage(this, "upper", v => Upper);
         manager.RegisterMessage(this, "lower", v => Lower);
         manager.RegisterMessage(this, "vowels", v => Vowels);
         manager.RegisterMessage(this, "uvowels", v => UpperVowels);
         manager.RegisterMessage(this, "lvowels", v => LowerVowels);
         manager.RegisterMessage(this, "cons", v => Consonants);
         manager.RegisterMessage(this, "ucons", v => UpperConsonants);
         manager.RegisterMessage(this, "lcons", v => LowerConsonants);
         manager.RegisterMessage(this, "digits", v => Digits);
         manager.RegisterMessage(this, "pi", v => Pi);
         manager.RegisterMessage(this, "e", v => Exp);
         manager.RegisterMessage(this, "punct", v => Punctuation);
         manager.RegisterMessage(this, "words", v => Words);
         manager.RegisterMessage(this, "spaces", v => Spaces);
         manager.RegisterMessage(this, "quotes", v => Quotes);
         manager.RegisterMessage(this, "degToRad", v => DegreeToRadian);
         manager.RegisterMessage(this, "recPat", v => RecordPattern);
         manager.RegisterMessage(this, "recSep", v => RecordSeparator);
         manager.RegisterMessage(this, "fldPat", v => FieldPattern);
         manager.RegisterMessage(this, "fldSep", v => FieldSeparator);
         manager.RegisterMessage(this, "id", v => Id);
         manager.RegisterMessage(this, "commaEnd", v => CommaEnd);
      }

      public static Value Tab => STRING_TAB;

      public static Value CRLF => STRING_CRLF;

      public static Value CR => STRING_CR;

      public static Value LF => STRING_LF;

      public static Value Letters => STRING_LETTERS;

      public static Value Upper => STRING_UPPER;

      public static Value Lower => STRING_LOWER;

      public static Value Vowels => STRING_VOWELS;

      public static Value UpperVowels => STRING_UVOWELS;

      public static Value LowerVowels => STRING_LVOWELS;

      public static Value Consonants => STRING_LCONSONANTS + STRING_UCONSONANTS;

      public static Value UpperConsonants => STRING_UCONSONANTS;

      public static Value LowerConsonants => STRING_LCONSONANTS;

      public static Value Digits => STRING_DIGITS;

      public static Value Pi => PI;

      public static Value Exp => E;

      public static Value Punctuation => STRING_PUNCT;

      public static Value Words => STRING_WORDS;

      public static Value Spaces => STRING_SPACES;

      public static Value Quotes => STRING_QUOTES;

      public static Value DegreeToRadian => PI / 180;

      public static Value RecordPattern => (Pattern)$"{STRING_BEGIN_PATTERN} '\r \n' | '\r' | '\n' {STRING_END_PATTERN}";

      public static Value RecordSeparator => "\r\n";

      public static Value FieldPattern => (Pattern)$"{STRING_BEGIN_PATTERN} + {STRING_END_PATTERN}";

      public static Value FieldSeparator => " ";

      public static Value Id => Id();

      public static Value CommaEnd => ",\r\n";
   }
}