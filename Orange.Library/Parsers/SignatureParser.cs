using System.Collections.Generic;
using System.Text;
using Orange.Library.Values;
using Orange.Library.Verbs;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Runtime;
using Standard.Types.Maybe;

namespace Orange.Library.Parsers
{
   public class SignatureParser : Parser, ITraitName
   {
      FreeParser freeParser;

      public SignatureParser(bool tabs)
         : base($"^ /(|{(tabs ? "tabs" : "sp")}|) /(('req' | 'optional') /s+) /(('func' | 'get' | 'set' |" +
            $" 'before' | 'after' | 'require' | 'ensure' | 'invariant') /s+) /({REGEX_VARIABLE}) /(['(:'])") =>
         freeParser = new FreeParser();

      public override Verb CreateVerb(string[] tokens)
      {
         var optionalLength = tokens[2].Length;
         var optional = tokens[2].Trim() == "optional";
         if (!InClassDefinition && optional)
            return null;

         Color(position, tokens[1].Length, Whitespaces);
         Color(optionalLength, KeyWords);
         var type = tokens[3].Trim();
         Color(tokens[3].Length, KeyWords);
         var name = tokens[4];
         var isStandard = tokens[5] == "(";
         Color(name.Length, isStandard ? Invokeables : Messaging);

         name = LongToMangledPrefix(type, name);

         Color(tokens[5].Length, Structures);
         var index = position + length;
         if (isStandard)
         {
            var parameterListParser = new ParameterListParser2();
            if (parameterListParser.Parse(source, index).If(out var list, out var newIndex))
            {
               var parameterCount = list.Count;
               var endParser = new EndParser();
               index = newIndex;
               if (endParser.Scan(source, index))
                  index = endParser.Position;
               overridePosition = index;
               result.Value = new Signature(name, parameterCount, optional);
               return new NullOp();
            }

            return null;
         }

         var builder = new StringBuilder();
         var messageParameterParser = new MessageParameterParser();
         var variableParser = new VariableParser();
         var parameterList = new List<Parameter>();
         if (variableParser.Scan(source, index))
         {
            var variable = (Variable)variableParser.Result.Value;
            var parameter = new Parameter(variable.Name);
            builder.Append(name);
            builder.Append("_");
            parameterList.Add(parameter);
            index = variableParser.Result.Position;
         }
         else
            return null;

         while (messageParameterParser.Scan(source, index))
         {
            var parameter = new Parameter(messageParameterParser.ParameterName);
            parameterList.Add(parameter);
            builder.Append(messageParameterParser.MessageName);
            builder.Append("_");
            index = messageParameterParser.Result.Position;
         }

         if (freeParser.Scan(source, index, REGEX_END1))
         {
            freeParser.ColorAll(Structures);
            overridePosition = freeParser.Position;
            name = builder.ToString();
            result.Value = new Signature(name, parameterList.Count, optional);
            return new NullOp();
         }

         return null;
      }

      public override string VerboseName => "signature";

      public string MemberName => ((Signature)result.Value).Name;

      public Lambda Getter => null;

      public Lambda Setter => null;
   }
}