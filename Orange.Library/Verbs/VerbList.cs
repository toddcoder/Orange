using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Orange.Library.Values;
using Standard.Types.Enumerables;
using Standard.Types.Strings;
using static Orange.Library.Managers.RegionManager;
using Standard.Types.RegularExpressions;

namespace Orange.Library.Verbs
{
   public class VerbList : IEnumerable<Verb>
   {
      List<Verb> verbs;
      int invokeCount;

      public VerbList(Block block)
      {
         verbs = block.AsAdded;
         invokeCount = verbs.Count(v => v is FunctionInvoke);
      }

      public VerbList()
      {
         verbs = new List<Verb>();
         invokeCount = 0;
      }

      public Verb this[int index]
      {
         get => verbs[index];
         set => verbs[index] = value;
      }

      public int InvokeCount => invokeCount;

      public int Count => verbs.Count;

      public void Add(Verb verb)
      {
         verbs.Add(verb);
         if (verb is FunctionInvoke)
            invokeCount++;
      }

      public void Add(VerbList verbList)
      {
         verbs.AddRange(verbList.verbs);
         invokeCount = verbs.Count(v => v is FunctionInvoke);
      }

      public void ReplacePlaceholderInvokeWithBody(VerbList verbList)
      {
         var result = new VerbList();
         var replaced = false;
         foreach (var verb in verbs)
            if (!replaced && verb is InvokePlaceholder)
            {
               result.Add(verbList);
               replaced = true;
            }
            else
               result.Add(verb);

         verbs = result.verbs;
      }

      public void ReplaceParameters(Parameters parameters)
      {
         var indexesToRemove = new List<int>();
         for (var i = 0; i < verbs.Count; i++)
         {
            var verb = verbs[i];
            switch (verb)
            {
               case Push push:
               {
                  var parameterName = push.Variable();
                  if (parameterName.IsSome && parameters.GetParameters()
                     .Any(p => p.Name == parameterName.Value || p.PlaceholderName == parameterName.Value))
                  {
                     var value = Regions[parameterName.Value];
                     verbs[i] = new Push(value);
                     continue;
                  }

                  if (push.Value is Block block)
                  {
                     var blockList = new VerbList(block);
                     blockList.ReplaceParameters(parameters);
                     var newBlock = blockList.Block;
                     newBlock.Expression = block.Expression;
                     verbs[i] = new Push(newBlock);
                  }
               }

                  break;
               case FunctionInvoke functionInvoke when parameters.GetParameters()
                  .Any(p => p.Name == functionInvoke.FunctionName || p.PlaceholderName == functionInvoke.FunctionName):
               {
                  var value = functionInvoke.Evaluate();
                  verbs[i] = new Push(value);
               }
                  continue;
               case Dereference _ when i > 0:
                  var lastIndex = i - 1;
                  var lastVerb = verbs[lastIndex];
                  if (lastVerb is Push lastPush)
                  {
                     var value = lastPush.Value;
                     value = Regions[value.Text];
                     verbs[i] = new Push(value);
                     indexesToRemove.Add(lastIndex);
                  }

                  break;
            }
         }

         indexesToRemove.Reverse();
         foreach (var i in indexesToRemove)
            verbs.RemoveAt(i);
      }

      public void ReplaceNameWithValue(string name, Value value)
      {
         var valuePush = new Push(value);
         for (var i = 0; i < verbs.Count; i++)
            if (verbs[i] is Push push)
            {
               var variableName = push.Variable();
               if (variableName.IsSome && variableName.Value == name)
               {
                  verbs[i] = valuePush;
                  continue;
               }

               if (push.Value is Block block)
               {
                  var innerList = new VerbList(block);
                  innerList.ReplaceNameWithValue(name, value);
                  var newBlock = innerList.Block;
                  newBlock.Expression = block.Expression;
                  verbs[i] = new Push(newBlock);
               }
            }
      }

      public void ReplaceArrayParameters(Parameters parameters)
      {
         foreach (var parameter in parameters.GetParameters())
         {
            if (!parameter.PlaceholderName.IsNotEmpty() || !parameter.PlaceholderName.Has(","))
               continue;

            foreach (var name in parameter.PlaceholderName.Split("','"))
            {
               var value = Regions[name];
               ReplaceNameWithValue(name, value);
            }
         }
      }

      public void ReplaceListParameters(Parameters parameters)
      {
         foreach (var parameter in parameters.GetParameters())
         {
            if (!parameter.PlaceholderName.IsNotEmpty() || !parameter.PlaceholderName.Has(":"))
               continue;

            foreach (var name in parameter.PlaceholderName.Split("':'"))
            {
               var value = Regions[name];
               ReplaceNameWithValue(name, value);
            }
         }
      }

      public bool ReplaceInvocationWithPlaceholder(string functionName, ref Arguments arguments)
      {
         var result = new VerbList();
         var replaced = false;
         foreach (var verb in verbs)
         {
            if (verb is FunctionInvoke functionInvoke)
            {
               if (functionInvoke.FunctionName == functionName)
                  if (replaced)
                     result.Add(functionInvoke);
                  else
                  {
                     var argumentsBlock = functionInvoke.Arguments.ArgumentsBlock;
                     result.Add(new InvokePlaceholder(functionInvoke.FunctionName, argumentsBlock.AsAdded));
                     var blocks = functionInvoke.Arguments.Blocks;
                     var newArguments = new Arguments();
                     foreach (var value in blocks.Select(block => block.Evaluate()))
                        newArguments.AddArgument(value);

                     arguments = newArguments;
                     replaced = true;
                  }
               else
               {
                  var funcResult = functionInvoke.Evaluate();
                  result.Add(new Push(funcResult));
               }

               continue;
            }

            result.Add(verb);
         }

         verbs = result.verbs;
         return replaced;
      }

      public VerbList[] Partition(string functionName)
      {
         var verbLists = new List<VerbList>();
         var newList = new VerbList();
         foreach (var verb in verbs)
         {
            if (verb is FunctionInvoke functionInvoke && functionInvoke.FunctionName == functionName)
            {
               newList.Add(functionInvoke);
               verbLists.Add(newList);
               newList = new VerbList();
               continue;
            }

            newList.Add(verb);
         }

         return verbLists.ToArray();
      }

      public bool IsEmpty => verbs.Count == 0;

      public Block Block => new Block(verbs);

      public IEnumerator<Verb> GetEnumerator() => verbs.GetEnumerator();

      public override string ToString() => verbs.Listify(" ");

      IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

      public Verb RemoveLastVerb()
      {
         var last = verbs.Count;
         if (last == 0)
            return null;

         last--;
         var result = verbs[last];
         verbs.RemoveAt(last);
         return result;
      }
   }
}