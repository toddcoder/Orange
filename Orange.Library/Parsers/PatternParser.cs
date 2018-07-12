using System.Collections.Generic;
using System.Linq;
using Orange.Library.Parsers.Conditionals;
using Orange.Library.Parsers.Patterns;
using Orange.Library.Patterns;
using Orange.Library.Replacements;
using Orange.Library.Values;
using Orange.Library.Verbs;
using static Orange.Library.Compiler;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Runtime;

namespace Orange.Library.Parsers
{
   public class PatternParser : Parser, IElementParser
   {
      bool resultElement;
      bool subPattern;

      public PatternParser(bool resultElement = false, bool subPattern = false)
         : base((subPattern ? "^ /(/s*)" : "^ /(' '*)") + REGEX_BEGIN_PATTERN)
      {
         IgnoreReplacement = false;
         this.resultElement = resultElement;
         this.subPattern = subPattern;
      }

      public override Verb CreateVerb(string[] tokens)
      {
         var parsers = new List<Parser>
         {
            new StringElementParser(),
            new FieldScanElementParser(),
            new ListElementParser(),
            new BalanceElementParser(),
            new LengthElementParser(),
            new FieldDelimiterElementParser(),
            new TabElementParser(),
            new RemElementParser(),
            new WordBoundaryElementParser(),
            new StringBoundaryElementParser(),
            new AtElementParser(),
            new AssertElementParser(),
            new BreakXElementParser(),
            new SpanElementParser(),
            new ArbNoElementParser(),
            new ArbElementParser(),
            new ClassElementParser(),
            new AlternateElementParser(),
            new OptionalElementParser(),
            new RangeElementParser(),
            new CountElementParser(),
            new PatternParser(true, true),
            new AnyElementParser(),
            new BlockElementParser(),
            new SingleCharacterElementParser(),
            new HexElementParser(),
            new FunctionElementParser(),
            new ConstantElementParser(),
            new VariableElementParser(),
            new EndPatternParser(IgnoreReplacement)
         };

         var index = position + length;
         Color(position, length, Structures);

         Element head = null;
         Element currentElement = null;

         var scanning = true;
         var sourceLength = source.Length;
         var isAlternate = false;
         var isOptional = false;
         IReplacement replacement = null;
         var not = false;
         var conditionalParser = new ConditionalParser();
         var replacementParser = new ReplacementParser();
         var found = false;

         while (scanning && index < sourceLength)
         {
            scanning = false;
            found = false;
            foreach (var parser in parsers.Where(parser => parser.Scan(source, index)))
            {
               if (parser is EndPatternParser endPatternParser)
               {
                  replacement = endPatternParser.Replacement;
                  index = endPatternParser.Result.Position;
                  found = true;
                  break;
               }

               if (parser is AlternateElementParser)
               {
                  isAlternate = true;
                  index = parser.Result.Position;
                  scanning = true;
                  found = true;
                  break;
               }

               if (parser is OptionalElementParser)
               {
                  isOptional = true;
                  index = parser.Result.Position;
                  scanning = true;
                  found = true;
                  break;
               }

               if (!(parser is IElementParser elementParser))
                  continue;

               var element = elementParser.Element;

               if (not)
               {
                  element.Not = true;
                  not = false;
               }

               if (element is NegateElement)
               {
                  not = true;
                  index = parser.Result.Position;
                  scanning = true;
                  found = true;
                  break;
               }

               element.ID = CompilerState.ObjectID();
               index = parser.Result.Position;

               if (replacementParser.Scan(source, index))
               {
                  element.Replacement = replacementParser.Replacement;
                  index = replacementParser.Result.Position;
               }

               if (conditionalParser.Scan(source, index))
                  index = conditionalParser.Result.Position;
               element.Conditional = conditionalParser.Conditional;

               if (isOptional)
               {
                  element.Alternate = new StringElement("");
                  isOptional = false;
               }

               if (element.AutoOptional)
                  element.Alternate = new StringElement("");

               if (isAlternate)
               {
                  currentElement.AppendAlternate(element);
                  isAlternate = false;
               }
               else
               {
                  if (head == null)
                     head = element;
                  else
                     currentElement.AppendNext(element);
                  currentElement = element;
               }
               scanning = true;
               found = true;
               break;
            }
         }

         Assert(found, "Pattern parser", $"Didn't understand pattern '{source.Substring(index)}'");

         if (head == null)
            head = new FailElement();

         var newPattern = new Pattern(head)
         {
            Replacement = replacement,
            LastElement = currentElement,
            SubPattern = subPattern
         };
         overridePosition = index;
         newPattern.Source = source.Substring(position, index - position).Trim();
         if (resultElement)
         {
            Element = new PatternElement(newPattern);
            return new NullOp();
         }

         result.Value = newPattern;
         return new Push(newPattern);
      }

      public override string VerboseName => "pattern";

      public bool IgnoreReplacement { get; set; }

      public Element Element { get; set; }
   }
}