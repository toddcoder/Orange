using System;
using Orange.Library.Verbs;
using Standard.Types.Collections;
using static System.Activator;
using static Orange.Library.Parsers.IDEColor.EntityType;

namespace Orange.Library.Parsers
{
   public class WordOperatorParser : Parser
   {
      static Hash<string, Type> operators;

      public static bool IsWordOperator(string word) => operators.ContainsKey(word);

      static WordOperatorParser() => operators = new Hash<string, Type>
      {
         ["xor"] = typeof(XOr),
         ["band"] = typeof(BitAnd),
         ["bor"] = typeof(BitOr),
         ["bxor"] = typeof(BitXOr),
         ["bnot"] = typeof(BitNot),
         ["shl"] = typeof(ShiftLeft),
         ["shr"] = typeof(ShiftRight),
         ["defer"] = typeof(Defer),
         ["skip"] = typeof(Skip),
         ["take"] = typeof(Take),
         ["for"] = typeof(For),
         ["if"] = typeof(If),
         //["plus"] = typeof(CountedRange),
         //["on"] = typeof(PushIndexerRange),
         ["zip"] = typeof(Zip),
         ["as"] = typeof(As),
         ["is"] = typeof(Is),
         ["in"] = typeof(In),
         ["sort"] = typeof(Sort),
         ["sortdesc"] = typeof(SortDesc),
         ["orderby"] = typeof(Order),
         ["group"] = typeof(Group),
         ["div"] = typeof(IntegerDivide),
         ["but"] = typeof(AppendToAlternation),
         ["unto"] = typeof(CreateUnto),
         ["map"] = typeof(Map),
         ["each"] = typeof(SelfMap),
         ["reduce"] = typeof(Reduce),
         ["foldl"] = typeof(FoldL),
         ["foldr"] = typeof(FoldR),
         ["during"] = typeof(During),
         ["while"] = typeof(TakeWhile),
         ["until"] = typeof(TakeUntil),
         ["shift"] = typeof(Shift),
         ["from"] = typeof(From),
         ["gcd"] = typeof(GreatestCommonDenominator),
         ["split"] = typeof(Split),
         ["join"] = typeof(Join),
         ["min"] = typeof(Min),
         ["max"] = typeof(Max),
         //["do"] = typeof(Do),
         ["all"] = typeof(All),
         ["any"] = typeof(AnyVerb),
         ["one"] = typeof(One),
         ["none"] = typeof(NoneVerb),
         ["x"] = typeof(Cross),
         ["xx"] = typeof(ArrayFromValue),
         ["divrem"] = typeof(DivRem)
      };

      public WordOperatorParser()
         : base("^ /s+ {a-z} /b") { }

      public override Verb CreateVerb(string[] tokens)
      {
         var message = tokens[0].Trim();
         var type = operators[message];
         if (type == null)
            return null; // new SimpleMessage(message, false, true);

         Color(position, length, KeyWords);
         var verb = (Verb)CreateInstance(type);
         verb.IsOperator = true;
         return verb;
      }

      public override string VerboseName => "word operator";
   }
}