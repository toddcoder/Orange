using Orange.Library.Values;
using Orange.Library.Verbs;
using static Orange.Library.Parsers.IDEColor.EntityType;

namespace Orange.Library.Parsers
{
   public class ArrayComprehensionParser : Parser
   {
      ArraySubComprehensionParser arraySubComprehensionParser;
      FreeParser freeParser;
      public ArrayComprehensionParser()
         : base("^ /(' '* '(') /'for' /b")
      {
         arraySubComprehensionParser = new ArraySubComprehensionParser();
         freeParser = new FreeParser();
      }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, tokens[1].Length, Structures);
         Color(tokens[2].Length, KeyWords);
         var index = NextPosition;
         Comprehension current = null;
         while (index < source.Length)
         {
            if (arraySubComprehensionParser.Scan(source, index))
            {
               index = arraySubComprehensionParser.Position;
               var comprehension = (Comprehension)arraySubComprehensionParser.Value;
               if (current == null)
                  current = comprehension;
               else
                  current.PushDownInnerComprehension(comprehension);
            }
            else
               return null;
            if (freeParser.Scan(source, index, "^ ' '* ','"))
            {
               index = freeParser.Position;
               freeParser.ColorAll(Structures);
               continue;
            }
            if (freeParser.Scan(source, index, "^ ' '* ')'") && current != null)
            {
               overridePosition = freeParser.Position;
               freeParser.ColorAll(Structures);
               result.Value = current;
               return new Push(current);
            }
            return null;
         }
         return null;
      }

      public override string VerboseName => "array comprehension";
   }
}