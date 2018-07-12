using System.Collections.Generic;
using Orange.Library.Values;
using Orange.Library.Verbs;
using Standard.Types.RegularExpressions;
using Standard.Types.Strings;

namespace Orange.Library
{
   public class EveryFillInBlock : EverySubBlock
   {
      static bool anyCandidate(Verb verb) => !(verb is Push) && verb.IsOperator;

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

      static string mangledVariableName(string index) => Runtime.VAR_MANGLE + index;

      static string mangledVariableName(int index) => mangledVariableName(index.ToString());

      const string REGEX_FILL_IN = "^ '$' /(/d+) $";

      List<Parameter> parameterList;

      public delegate void AddParameterHandler(Parameter parameter);

      public event AddParameterHandler AddParameter;

      public EveryFillInBlock(Block block)
         : base(block) => parameterList = new List<Parameter>();

      public EveryFillInBlock(Block block, List<Parameter> parameterList)
         : base(block) => this.parameterList = parameterList;

      public override Block PushBlock(Block sourceBlock)
      {
         var builder = new CodeBuilder();
         var matcher = new Matcher();

         foreach (var verb in sourceBlock.AsAdded)
         {
            if (verb is Push push)
            {
               var variableName = push.Variable();
               if (variableName.IsSome)
               {
                  if (variableName.Value == "_")
                  {
                     createVariable(parameterList, builder);
                     AddParameter?.Invoke(parameterList[parameterList.Count - 1]);
                     continue;
                  }

                  if (matcher.IsMatch(variableName.Value, REGEX_FILL_IN))
                  {
                     var index = matcher[0, 1].ToInt();
                     createVariable(parameterList, builder, index);
                     AddParameter?.Invoke(parameterList[index]);
                     continue;
                  }
               }
            }

            builder.Verb(verb);
         }

         return builder.Block;
      }

      public override (Block, Parameters, bool) ClosureBlock(Block sourceBlock)
      {
         var verbs = sourceBlock.AsAdded;
         if (verbs.Count == 0)
            return (null, new NullParameters(), false);

         if (anyCandidate(verbs[0]))
            verbs.Insert(0, pushAnyVariable());
         var lastIndex = verbs.Count - 1;
         if (anyCandidate(verbs[lastIndex]))
            verbs.Add(pushAnyVariable());

         var newParameters = new List<Parameter>();

         AddParameter += newParameters.Add;
         var returnBlock = PushBlock(sourceBlock);
         AddParameter -= newParameters.Add;
         var someParameters = new Parameters(newParameters);
         return (returnBlock, someParameters, someParameters.Splatting);
      }

      static Push pushAnyVariable() => new Push(new Variable("_"));

      public override Block ArgumentsBlocks(Block sourceBlock) => PushBlock(sourceBlock);

      public List<Parameter> Parameters => parameterList;
   }
}