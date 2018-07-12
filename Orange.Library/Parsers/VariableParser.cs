using Orange.Library.Values;
using Orange.Library.Verbs;
using Standard.Types.RegularExpressions;
using Standard.Types.Strings;
using static Orange.Library.Compiler;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Runtime;

namespace Orange.Library.Parsers
{
   public class VariableParser : Parser
   {
      public VariableParser()
         : base($"^ /(|sp|) /({REGEX_VARIABLE}) /'*'? '`'?")
      {
      }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, tokens[1].Length, Whitespaces);
         var variableName = tokens[2];
         if (CompilerState.IsRegisteredOperator(variableName))
         {
            Color(tokens[2].Length, Operators);
            return CompilerState.Operator(variableName);
         }
         var star = tokens[3];
         var entityType = Variables;
         if (source.Skip(position + length).IsMatch("^ ['([']"))
            entityType = Invokeables;
         if (IsClassName(variableName))
            entityType = Types;
         Color(variableName.Length, entityType);
         Color(star.Length, Operators);
         if (star == "*")
            variableName += "_" + CompilerState.ObjectID();
         var variable = new Variable(variableName);
         result.Value = variable;
         return new Push(variable);
      }

      public override string VerboseName => "variable";
   }
}