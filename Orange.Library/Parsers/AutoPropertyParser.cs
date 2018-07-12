using Orange.Library.Values;
using Orange.Library.Verbs;
using Standard.Types.Strings;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Runtime;

namespace Orange.Library.Parsers
{
   public class AutoPropertyParser : Parser, ITraitName
   {
      string propertyName;
      Lambda getter;
      Lambda setter;

      public AutoPropertyParser()
         : base($"^ /([' ' /t]*) /('auto' | 'readonly') /(/s+) /({REGEX_VARIABLE})? -(> '(' )") { }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, tokens[1].Length, Whitespaces);
         var type = tokens[2];
         Color(type.Length, KeyWords);
         Color(tokens[3].Length, Whitespaces);
         propertyName = tokens[4];
         Assert(propertyName.IsNotEmpty(), "Create auto property", "Auto property has no name (remove parentheses)");
         Color(propertyName.Length, Variables);
         var readOnly = type == "readonly";
         var creator = new CreateAutoProperty.AutoPropertyCreator(propertyName, readOnly);
         getter = creator.GetLambda;
         setter = creator.SetLambda;
         result.Value = null;
         return new CreateAutoProperty(propertyName, readOnly);
      }

      public override string VerboseName => "auto property";

      public string MemberName => propertyName;

      public Lambda Getter => getter;

      public Lambda Setter => setter;
   }
}