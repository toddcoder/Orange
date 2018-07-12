using Orange.Library.Parsers.Special;
using Orange.Library.Values;
using Orange.Library.Verbs;
using Standard.Types.Maybe;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Runtime;

namespace Orange.Library.Parsers
{
   public class TypeParser : Parser
   {
      public TypeParser()
         : base($"^ /(|tabs| 'type' /s+) /({REGEX_VARIABLE}) /(/s* '(')?") { }

      public override Verb CreateVerb(string[] tokens)
      {
         if (!InClassDefinition)
            return null;

         Color(position, tokens[1].Length, KeyWords);
         var name = tokens[2];
         Color(name.Length, Variables);
         var parameterLength = tokens[3].Length;
         Color(parameterLength, Structures);
         var index = position + length;
         var parameters = new Parameters();
         if (parameterLength > 0)
         {
            var parametersParser = new ParametersParser();
            var newIndex = parametersParser.Parse(source, index);
            if (!newIndex.If(out parameters, out index))
               return null;
         }

         overridePosition = index;

         var builder = new CodeBuilder();
         if (parameters.Length == 0)
         {
            var mangledName = MangledName(name);
            var cls = new Class(parameters, objectBlock(name), new Block(), ClassName, new string[0], new Parameters(), false);
            var verb = new CreateClass(mangledName, cls);
            builder.Verb(verb);
            builder.End();
            builder.Push();
            builder.FunctionInvoke(mangledName);
            builder.AssignToNewField(true, name, builder.Pop(true), global: true);
            builder.End();
         }
         else
         {
            var cls = new Class(parameters, objectBlock(name), new Block(), ClassName, new string[0], new Parameters(), false);
            var verb = new CreateClass(name, cls);
            builder.Verb(verb);
         }
         if (HelperBlock == null)
            HelperBlock = new Block();
         foreach (var verb in builder.Block.AsAdded)
            HelperBlock.Add(verb);

         return new NullOp();
      }

      public override string VerboseName => "type";

      static Block objectBlock(string name)
      {
         var builder = new CodeBuilder();
         builder.Push();
         builder.Value(name);
         var lambda = builder.Lambda();
         builder.Pop(true);
         builder.Function(LongToMangledPrefix("get", "name"), lambda);
         return builder.Block;
      }
   }
}