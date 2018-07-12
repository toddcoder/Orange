using System.Collections.Generic;
using System.Linq;
using Orange.Library.Values;
using Orange.Library.Verbs;
using Standard.Types.Collections;
using Standard.Types.Maybe;
using Standard.Types.RegularExpressions;
using Standard.Types.Strings;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Runtime;
using static Orange.Library.Verbs.Verb.AffinityType;
using static Orange.Library.Parsers.ExpressionParser;
using static Orange.Library.Parsers.Stop;

namespace Orange.Library.Parsers
{
   public class FillInBlockParser : Parser
   {
      const string REGEX_FILL_IN = "^ '$' /(/d+) $";

      public FillInBlockParser()
         : base("^ /s* '$('") { }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, length, Structures);
         if (GetExpression(source, NextPosition, CloseParenthesis()) is Some<(Block, int)> some)
         {
            (var sourceBlock, var index) = some.Value;
            (var block, var parameters) = FillInBlock(sourceBlock);
            var executable = block ?? sourceBlock;
            result.Value = new Lambda(new Region(), executable, parameters, false);
            overridePosition = index;
            return new CreateLambda(parameters, executable, false);
         }

         return null;
      }

      public override string VerboseName => "Fill in block";

      static bool bothCandidate(Verb verb) => verb.IsOperator && verb.OperandCount == 2 && verb.Affinity == Infix;

      static bool beforeCandidate(Verb verb) => verb.IsOperator && verb.OperandCount == 1 && verb.Affinity == Postfix;

      public static (Block, Parameters) FillInBlock(Block sourceBlock)
      {
         var followable = new Set<Verb.AffinityType> { Prefix };
         var verbs = new VerbList(sourceBlock);
         if (verbs.IsEmpty)
            return ((Block)null, new Parameters());

         var newVerbs = new VerbList();
         var lastIndex = verbs.Count - 1;
         for (var i = 0; i < verbs.Count; i++)
         {
            var verb = verbs[i];
            if (verb.Affinity == Verb.AffinityType.Value)
            {
               newVerbs.Add(verb);
               continue;
            }

            var lastVerb = newVerbs.Count == 0 ? null : newVerbs[newVerbs.Count - 1];
            var isFirst = i == 0;
            var isLast = i == lastIndex;

            switch (verb.Affinity)
            {
               case Postfix:
               case Infix:
                  if (isFirst || followable.Contains(lastVerb.Affinity))
                     newVerbs.Add(push());
                  break;
            }

            newVerbs.Add(verb);

            switch (verb.Affinity)
            {
               case Prefix:
               case Infix:
                  if (isLast || followable.Contains(verbs[i + 1].Affinity))
                     newVerbs.Add(push());
                  break;
            }
         }

         verbs = newVerbs;

         var parameterList = new List<Parameter>();
         var builder = new CodeBuilder();
         var matcher = new Matcher();

         foreach (var verb in verbs)
         {
            if (verb is Push push)
            {
               var variableName = push.Variable();
               if (variableName.IsSome)
               {
                  if (variableName.Value == "_")
                  {
                     createVariable(parameterList, builder);
                     continue;
                  }

                  if (matcher.IsMatch(variableName.Value, REGEX_FILL_IN))
                  {
                     var index = matcher[0, 1].ToInt();
                     createVariable(parameterList, builder, index);
                     continue;
                  }
               }

               if (push.Value is Block block)
               {
                  (var innerBlock, var innerParameters) = FillInBlock(block);
                  if (innerBlock == null)
                     continue;

                  foreach (var parameter in innerParameters.GetParameters()
                     .Where(parameter => parameterList.All(p => p.Name != parameter.Name)))
                     parameterList.Add(parameter);

                  builder.Value(innerBlock);
                  continue;
               }
            }

            switch (verb)
            {
               case Invoke invoke:
               {
                  var argumentsBlock = invoke.Arguments.ArgumentsBlock;
                  (var innerBlock, var innerParameters) = FillInBlock(argumentsBlock);
                  if (innerBlock == null)
                     continue;

                  foreach (var parameter in innerParameters.GetParameters()
                     .Where(parameter => parameterList.All(p => p.Name != parameter.Name)))
                     parameterList.Add(parameter);

                  builder.Invoke(new Arguments(innerBlock));
                  continue;
               }
               case PushIndexer pushIndexer:
               {
                  var indexes = pushIndexer.Indexes;
                  (var innerBlock, var innerParameters) = FillInBlock(indexes);
                  if (innerBlock == null)
                     continue;

                  foreach (var parameter in innerParameters.GetParameters()
                     .Where(parameter => parameterList.All(p => p.Name != parameter.Name)))
                     parameterList.Add(parameter);

                  builder.PushIndexer(innerBlock);
                  continue;
               }
            }

            builder.Verb(verb);
         }

         return (builder.Block, new Parameters(parameterList));
      }

      static Push push() => new Push(new Variable("_"));

      static void createVariable(List<Parameter> parameterList, CodeBuilder builder)
      {
         var index = parameterList.Count;
         createVariable(parameterList, builder, index);
      }

      static void createVariable(List<Parameter> parameterList, CodeBuilder builder, int index)
      {
         builder.Variable(mangledVariableName(index));
         var count = parameterList.Count;

         if (index < count)
            return;

         var stack = new Stack<Parameter>();
         for (var i = index; i >= count; i--)
            stack.Push(new Parameter(mangledVariableName(i)));
         while (stack.Count > 0)
            parameterList.Add(stack.Pop());
      }

      static string mangledVariableName(string index) => VAR_MANGLE + index;

      static string mangledVariableName(int index) => mangledVariableName(index.ToString());
   }
}