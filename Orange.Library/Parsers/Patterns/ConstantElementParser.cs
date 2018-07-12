using Orange.Library.Patterns;
using Orange.Library.Verbs;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Runtime;

namespace Orange.Library.Parsers.Patterns
{
   public class ConstantElementParser : Parser, IElementParser
   {
      public ConstantElementParser()
         : base($"^ /(/s* '`') /({REGEX_VARIABLE})")
      {
      }

      public override Verb CreateVerb(string[] tokens)
      {
         var name = tokens[2];
         Color(position, length, Variables);

         var builder = new CodeBuilder();
         builder.Variable("Const");
         builder.SendMessage(name);
         Element = new BlockElement2(builder.Block);

         return new NullOp();
      }

      public override string VerboseName => "Const";

      public Element Element
      {
         get;
         set;
      }
   }
}