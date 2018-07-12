using System;
using Orange.Library.Verbs;
using Standard.Types.Collections;
using static System.Activator;
using static Orange.Library.Parsers.IDEColor.EntityType;
using Format = Orange.Library.Verbs.Format;

namespace Orange.Library.Parsers
{
   public class TwoCharacterOperatorParser : Parser
   {
      public const string REGEX_PUNCTUATION = @"['+*//%=!<>&|@#~.,\^?;:-']";

      static Hash<string, Type> operators;

      public static Type Operator(string name) => operators[name];

      static TwoCharacterOperatorParser() => operators = new Hash<string, Type>
      {
         ["+"] = typeof(Add),
         ["-"] = typeof(Subtract),
         ["*"] = typeof(Multiply),
         ["/"] = typeof(Divide),
         ["%"] = typeof(Mod),
         ["%%"] = typeof(DivBy),
         ["!%"] = typeof(NotDivBy),
         ["**"] = typeof(Power),
         ["="] = typeof(Bind),
         ["=="] = typeof(Equals),
         ["!="] = typeof(NotEqual),
         [">"] = typeof(GreaterThan),
         [">="] = typeof(GreaterThanEqual),
         ["<"] = typeof(LessThan),
         ["<="] = typeof(LessThanEqual),
         [";"] = typeof(CreateTuple),
         ["+="] = typeof(AddAssign),
         ["-="] = typeof(SubtractAssign),
         ["*="] = typeof(MultiplyAssign),
         ["/="] = typeof(DivideAssign),
         ["**="] = typeof(PowerAssign),
         [","] = typeof(AppendToArray),
         ["=>"] = typeof(PushKeyedValue),
         ["~"] = typeof(Concatenation),
         ["~="] = typeof(ConcatenationAssign),
         ["<=>"] = typeof(NumericComparison),
         ["<&>"] = typeof(CompareAnd),
         ["|"] = typeof(Apply),
         ["||"] = typeof(ApplyWhile),
         ["|?"] = typeof(ApplyIf),
         ["!|"] = typeof(ApplyNot),
         ["\\"] = typeof(Format),
         ["=:"] = typeof(NewMessage),
         ["~~"] = typeof(ConcatenateSeparated),
         ["??"] = typeof(DefaultTo),
         ["??="] = typeof(DefaultToAssign),
         ["."] = typeof(FunctionApply),
         ["<<<"] = typeof(ShiftL),
         [">>>"] = typeof(ShiftR),
         ["!-"] = typeof(SetOption),
         [":=:"] = typeof(EndOfDataAssign),
         ["==="] = typeof(Identical),
         ["!=="] = typeof(NotIdentical),
         ["&&"] = typeof(AddToConcatenation),
         ["<-"] = typeof(Generate),
         ["^^"] = typeof(AppendToSet),
         ["|>"] = typeof(Pipe),
         ["<|"] = typeof(Unpipe),
         ["::"] = typeof(Cons),
         [".."] = typeof(NSRange),
         ["..."] = typeof(NSExclusiveRange),
         [".+"] = typeof(NSBy),
         [".-"] = typeof(NSNegBy),
         [".*"] = typeof(NSLazyRange),
         [".:"] = typeof(NSCountedRange),
         ["->"] = typeof(ImmediateMap),
         ["/:"] = typeof(FoldL),
         [":\\"] = typeof(FoldR),
         ["<<"] = typeof(Take),
         [">>"] = typeof(Skip),
         ["<>"] = typeof(Scan),
         ["><"] = typeof(FlatCross),
         ["=~"] = typeof(IsMatch),
         ["!~"] = typeof(IsNotMatch)
      };

      public TwoCharacterOperatorParser()
         : base($"^ /(|sp|) /({REGEX_PUNCTUATION} 2)") { }

      public override Verb CreateVerb(string[] tokens)
      {
         var type = operators[tokens[2]];
         if (type == null)
            return null;

         Color(position, tokens[1].Length, Whitespaces);
         Color(tokens[2].Length, Operators);
         var verb = (Verb)CreateInstance(type);
         verb.IsOperator = true;
         return verb;
      }

      public override string VerboseName => "operator";
   }
}