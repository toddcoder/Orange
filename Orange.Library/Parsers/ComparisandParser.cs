using Orange.Library.Parsers.Special;
using Orange.Library.Values;
using Orange.Library.Verbs;
using Standard.Types.Collections;
using Standard.Types.Maybe;
using static Orange.Library.Compiler;
using static Orange.Library.Runtime;
using Array = Orange.Library.Values.Array;
using If = Orange.Library.Verbs.If;
using SendMessage = Orange.Library.Verbs.SendMessage;

namespace Orange.Library.Parsers
{
   public static class ComparisandParser
   {
      public static IMaybe<(Block, Block, int)> GetComparisand(string source, int index, Stop stop)
      {
         if (ExpressionParser.GetExpression(source, index, stop).If(out var expression, out var i))
         {
            (var comparisand, var condition) = GetComparisand(expression);
            return (comparisand, condition, i).Some();
         }

         return MaybeFunctions.none<(Block, Block, int)>();
      }

      public static (Block, Block) GetComparisand(Block block)
      {
         var variable = new Variable();
         var builder = new CodeBuilder();
         var bindingName = "";
         var bindNextValue = false;
         var iffing = false;
         Block condition = null;

         foreach (var verb in block.AsAdded)
         {
            if (iffing)
            {
               condition.Add(verb);
               continue;
            }

            Value value;
            string name;
            Arguments arguments;
            switch (verb)
            {
               case Bind _:
                  builder.PopLastVerb();
                  bindingName = variable?.Name ?? "_";
                  bindNextValue = true;
                  break;
               case If _:
                  iffing = true;
                  condition = new Block();
                  break;
               case ToList toList:
                  var list = new List();
                  var listBlock = toList.Block;
                  foreach (var listVerb in listBlock.AsAdded)
                  {
                     switch (listVerb)
                     {
                        case Push listPush:
                           value = listPush.Value;
                           break;
                        case PushArrayLiteral pushArrayLiteral:
                           value = pushArrayLiteral.Array;
                           break;
                        default:
                           continue;
                     }

                     if (value is Variable listVariable)
                     {
                        name = listVariable.Name;
                        value = evaluateVariable(name);
                     }
                     list = list.Add(value);
                  }

                  builder.Value(list);
                  break;
               case SendMessage sendMessage:
                  var lastVerb = builder.Last;
                  if (lastVerb is Push push1 && push1.Value is Placeholder placeholder)
                  {
                     builder.PopLastVerb();
                     builder.Variable(placeholder.Text);
                  }
                  arguments = sendMessage.Arguments;
                  arguments = convertArguments(arguments);
                  builder.SendMessage(sendMessage.Message, arguments);
                  break;
               case Or or:
                  var expression = or.Expression;
                  (var expressionBlock, _) = GetComparisand(expression);
                  builder.Verb(new AppendToAlternation());
                  builder.Inline(expressionBlock);
                  break;
               case CreateRecord createRecord:
                  var members = createRecord.Members;
                  var newMembers = new Hash<string, Thunk>();
                  foreach (var item in members)
                  {
                     (var thunkBlock, _) = GetComparisand(item.Value.Block);
                     newMembers[item.Key] = new Thunk(thunkBlock, item.Value.Region);
                  }

                  builder.Verb(new CreateRecord(newMembers, createRecord.FieldName));
                  break;
               case Push push:
                  value = push.Value;

                  switch (value)
                  {
                     case Variable variable1:
                        name = variable1.Name;
                        value = evaluateVariable(name);
                        break;
                     case Array array:
                        var newArray = new Array();
                        foreach (var item in array)
                        {
                           if (item.Value is Variable variable1)
                           {
                              name = variable1.Name;
                              value = evaluateVariable(name);
                           }
                           else
                              value = item.Value;
                           newArray.Add(value);
                        }

                        value = newArray;
                        break;
                     case Block pushedBlock:
                        (var retrievedBlock, _) = GetComparisand(pushedBlock);
                        value = retrievedBlock;
                        break;
                  }

                  if (bindNextValue)
                  {
                     builder.Verb(new BindValue(bindingName, value));
                     bindNextValue = false;
                  }
                  else
                     builder.Value(value);
                  break;
               case PushSome pushSome:
                  var someBlock = pushSome.Expression;
                  (someBlock, _) = GetComparisand(someBlock);
                  builder.Verb(new PushSome(someBlock));
                  break;
               case FunctionInvoke functionInvoke:
                  var functionName = functionInvoke.FunctionName;
                  arguments = functionInvoke.Arguments;
                  arguments = convertArguments(arguments);
                  builder.FunctionInvoke(functionName, arguments);
                  break;
               default:
                  builder.Verb(verb);
                  break;
            }
         }

         if (condition != null)
            condition.AutoRegister = false;
         return (builder.Block, condition);
      }

      static Value evaluateVariable(string name)
      {
         if (name == "_")
            return new Any();

         var possibleClass = CompilerState.Class(name);
         if (possibleClass.IsSome)
            return possibleClass.Value;

         var possibleTrait = CompilerState.Trait(name);
         if (possibleTrait.IsSome)
            return possibleTrait.Value;

         if (IsClassName(name))
            return new Variable(name);

         return new Placeholder(name);
      }

      static Arguments convertArguments(Arguments arguments)
      {
         var patternParameterParser = new PatternParameterListParser();
         if (patternParameterParser.Parse(arguments.ArgumentsBlock))
         {
            var list = patternParameterParser.List;
            var builder = new CodeBuilder();
            foreach (var parameter in list)
               builder.Argument(parameter.Comparisand);

            return builder.Arguments;
         }

         return arguments;
      }
   }
}