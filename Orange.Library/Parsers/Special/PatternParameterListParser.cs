using System.Collections.Generic;
using System.Linq;
using Orange.Library.Values;
using Orange.Library.Verbs;
using Standard.Types.Maybe;
using Standard.Types.RegularExpressions;
using Standard.Types.Strings;
using static Orange.Library.Parsers.ExpressionParser;
using static Orange.Library.Parsers.Stop;
using static Standard.Types.Maybe.MaybeFunctions;
using Array = Orange.Library.Values.Array;
using If = Orange.Library.Verbs.If;

namespace Orange.Library.Parsers.Special
{
   public class PatternParameterListParser : SpecialParser<List<Parameter>>, IReturnsParameterList
   {
      Stop stop;
      List<Parameter> list;
      Block condition;

      public PatternParameterListParser()
      {
         stop = CloseBracket();
         list = new List<Parameter>();
         condition = null;
      }

      public PatternParameterListParser(Stop stop)
      {
         this.stop = stop;
         list = new List<Parameter>();
         condition = null;
      }

      public override IMaybe<(List<Parameter>, int)> Parse(string source, int index)
      {
         return GetExpression(source, index, stop).Map(t => when(Parse(t.Item1), () => (list, t.Item2)));
      }

      public bool Parse(Block block)
      {
         var variable = new Variable();
         condition = null;
         Multi = true;

         string bindingName;
         if (block.Count == 0)
         {
            bindingName = "";
            list.Add(getParameter(0, new Any(), ref bindingName));
            return true;
         }

         var addingToCondition = false;

         bindingName = "";
         foreach (var verb in block.AsAdded)
         {
            if (addingToCondition)
            {
               condition.Add(verb);
               continue;
            }

            switch (verb)
            {
               case Push push:
                  var value = push.Value;
                  switch (value)
                  {
                     case Variable variable1:
                        var name = variable1.Name;
                        value = name == "_" ? (Value)new Any() : new Placeholder(name);
                        break;
                     case Block arrayBlock when arrayBlock.AsAdded.Any(v => v is AppendToArray):
                        value = ReplacePlaceholders(arrayBlock);
                        break;
                  }

                  list.Add(getParameter(list.Count, value, ref bindingName));
                  break;
               case PushArrayLiteral pushArrayLiteral:
                  var array = pushArrayLiteral.Array;
                  list.Add(getParameter(list.Count, array, ref bindingName));
                  break;
               case FunctionInvoke functionInvoke:
               {
                  var functionName = functionInvoke.FunctionName;
                  var arguments = functionInvoke.Arguments;
                  var innerParser = new PatternParameterListParser();
                  if (innerParser.Parse(arguments.ArgumentsBlock))
                  {
                     var sublist = innerParser.List;
                     var builder = new CodeBuilder();
                     foreach (var parameter in sublist)
                        builder.Argument(parameter.Comparisand);

                     builder.FunctionInvoke(functionName);
                     list.Add(getParameter(list.Count, builder.Block, ref bindingName));
                  }

                  break;
               }
               case CreateLambda createLambda:
                  var closure = (Lambda)createLambda.Evaluate();
                  list.Add(getParameter(list.Count, closure, ref bindingName));
                  break;
               case SendMessage sendMessage when variable != null:
               {
                  var variableName = variable.Name;
                  var arguments = sendMessage.Arguments;
                  removeLastParameter(list, ref bindingName);
                  var innerParser = new PatternParameterListParser();
                  if (innerParser.Parse(arguments.ArgumentsBlock))
                  {
                     var sublist = innerParser.List;
                     var builder = new CodeBuilder();
                     foreach (var parameter in sublist)
                        builder.Argument(parameter.Comparisand);

                     builder.Variable(variableName);
                     builder.SendMessage(sendMessage.Message, builder.Arguments);
                     list.Add(getParameter(list.Count, builder.Block, ref bindingName));
                  }

                  break;
               }
               case SendMessageToClass sendMessageToClass:
               {
                  var arguments = sendMessageToClass.Arguments;
                  var innerParser = new PatternParameterListParser();
                  if (innerParser.Parse(arguments.ArgumentsBlock))
                  {
                     var sublist = innerParser.List;
                     var builder = new CodeBuilder();
                     foreach (var parameter in sublist)
                        builder.Argument(parameter.Comparisand);

                     builder.SendMessageToClass(sendMessageToClass.Message, builder.Arguments);
                     list.Add(getParameter(list.Count, builder.Block, ref bindingName));
                  }

                  break;
               }
               case Bind _ when variable != null:
                  list.RemoveAt(list.Count - 1);
                  bindingName = variable.Name;
                  break;
               case CreateSet createSet:
                  list.Add(getParameter(list.Count, createSet.Create(), ref bindingName));
                  break;
               case If _:
                  addingToCondition = true;
                  condition = new Block();
                  break;
               case ToList toList:
                  var newlist = ReplacePlaceholdersInList(toList.Block);
                  list.Add(getParameter(list.Count, newlist, ref bindingName));
                  break;
            }
         }

         if (condition != null)
            condition.AutoRegister = false;
         return true;
      }

      public bool Multi { get; set; }

      public bool Currying { get; set; }

      public Block Condition => condition;

      static Parameter getParameter(int index, Value comparisand, ref string bindingName)
      {
         var block = CodeBuilder.PushValue(comparisand);
         return getParameter(index, block, ref bindingName);
      }

      static Parameter getParameter(int index, Block block, ref string bindingName)
      {
         var name = bindingName.IsNotEmpty() ? bindingName : "$" + index;
         bindingName = "";
         return new Parameter(name, comparisand: block, defaultValue: CodeBuilder.PushValue(new Nil()));
      }

      public static Array ReplacePlaceholders(Block block)
      {
         var array = new Array();
         foreach (var verb in block.AsAdded)
         {
            Value value;
            switch (verb)
            {
               case Push push:
                  value = push.Value;
                  break;
               case PushArrayLiteral pushArrayLiteral:
                  value = pushArrayLiteral.Array;
                  break;
               default:
                  continue;
            }

            if (value is Variable variable)
            {
               var name = variable.Name;
               value = name == "_" ? (Value)new Any() : new Placeholder(name);
            }
            array.Add(value);
         }

         return array;
      }

      public static List ReplacePlaceholdersInList(Block listBlock)
      {
         var list = new List();
         foreach (var verb in listBlock.AsAdded)
         {
            Value value;
            switch (verb)
            {
               case Push push:
                  value = push.Value;
                  break;
               case PushArrayLiteral pushArrayLiteral:
                  value = pushArrayLiteral.Array;
                  break;
               default:
                  continue;
            }

            if (value is Variable variable)
            {
               var name = variable.Name;
               value = name == "_" ? (Value)new Any() : new Placeholder(name);
            }
            list = list.Add(value);
         }

         return list;
      }

      static void removeLastParameter(List<Parameter> parameters, ref string bindingName)
      {
         var lastIndex = parameters.Count - 1;
         var parameter = parameters[lastIndex];
         if (!parameter.Name.IsMatch("^ '$' /d+ $"))
            bindingName = parameter.Name;
         parameters.RemoveAt(lastIndex);
      }

      public List<Parameter> List => list;
   }
}