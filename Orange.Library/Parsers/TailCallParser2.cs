using System;
using Orange.Library.Values;
using Orange.Library.Verbs;
using Standard.Types.Collections;
using Standard.Types.Maybe;
using Standard.Types.Objects;
using Standard.Types.RegularExpressions;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Parsers.StatementParser;
using static Orange.Library.Parsers.Parser;
using static Orange.Library.Runtime;
using static Standard.Types.Tuples.TupleFunctions;
using Array = Orange.Library.Values.Array;
using When = Orange.Library.Verbs.When;

namespace Orange.Library.Parsers
{
   public class TailCallParser2
   {
      const string LOCATION = "Tail call parser";

      Matcher matcher;

      public TailCallParser2()
      {
         matcher = new Matcher();
      }

      public IMaybe<Tuple<int, Block>> Parse(string source, int index, Parameters parameters)
      {
         Block checkExpresion;
         if (!parseitem(source, "^ /(/s* 'return' /s+ 'if') /(/s* '(')", index).Assign(out index, out checkExpresion))
            return new None<Tuple<int, Block>>();

         Block finalValue;
         if (!parseitem(source, "^ /(/s* 'then') /(/s* '(')", index).Assign(out index, out finalValue))
            return new None<Tuple<int, Block>>();

         Block block;
         if (!parseitem(source, "^ /(/s* 'else') /(/s* '(')", index).Assign(out index, out block))
            return new None<Tuple<int, Block>>();

         Block invariant;
         Block increment;
         Assert(IsTailCall(block).Assign(out invariant, out increment), LOCATION, "Not a tail-call candidate");
         string accumName;
         if (invariant.Count == 1 && NameFromVariable(invariant[0]).Assign(out accumName) && accumName == VAR_ACCUM)
            invariant = (Block)finalValue.Clone();

         var tailCallBlock = ConvertBlocks(parameters, finalValue, checkExpresion, invariant, increment);

         return tuple(index, tailCallBlock).Some();
      }

      public static Block ConvertBlocks(Parameters parameters, Block finalValue, Block checkExpresion, Block invariant,
         Block increment, Array test = null)
      {
         var builder = new CodeBuilder();
         builder.Define(VAR_ACCUM);
         builder.Assign();
         builder.Parenthesize(finalValue);
         builder.End();

         builder.Push();

         builder.Push();
         builder.Exit();
         var ifThen = builder.Pop();

         builder.If(checkExpresion, ifThen);

         builder.Variable(VAR_ACCUM);
         builder.Assign();
         builder.Parenthesize(invariant);
         builder.End();

         var setup = true;
         var parameterIndex = 0;
         foreach (var verb in increment)
         {
            if (setup)
            {
               var length = parameters.VariableNames.Length;
               Assert(parameterIndex < length, LOCATION, "No more parameters");
               builder.Define(VAR_MANGLE + parameters[parameterIndex++].Name);
               builder.Assign();
               setup = false;
            }
            if (verb is AppendToArray)
            {
               builder.End();
               setup = true;
            }
            else
               builder.Verb(verb);
         }

         builder.End();

         foreach (var variableName in parameters.VariableNames)
         {
            builder.Variable(variableName);
            builder.Assign();
            builder.Variable(VAR_MANGLE + variableName);
            builder.End();
         }

         var actions = builder.Pop();

         builder.Push();
         if (test == null)
            builder.Value(true);
         else
         {
            builder.Variable(parameters[0].Name);
            builder.Verb(new When());
            builder.Value(test);
         }
         var whileCondition = builder.Pop();
         builder.While(whileCondition, actions);
         builder.End();

         builder.Return();
         if (finalValue.Count == 1)
            if (NameFromVariable(finalValue[0]).IsSome)
               builder.Parenthesize(finalValue);
            else
               builder.Variable(VAR_ACCUM);
         else
            builder.Variable(VAR_ACCUM);
         builder.End();

         return builder.Block;
      }

      IMaybe<Tuple<int, Block>> parseitem(string source, string pattern, int index)
      {
         if (matcher.IsMatch(source.Substring(index), pattern))
         {
            Color(index, matcher[0, 1].Length, KeyWords);
            Color(matcher[0, 2].Length, Structures);
            index += matcher[0].Length;
            return GetBlock(source, index, true).Map(t => tuple(t.Item2, t.Item1));
         }
         return new None<Tuple<int, Block>>();
      }

      public static IMaybe<string> NameFromVariable(Verb verb) => verb.As<Push>()
         .Map(push => push.Value.As<Variable>())
         .Map(variable => variable.Name);

      public static IMaybe<Tuple<Block, Block>> IsTailCall(Block block)
      {
         var count = block.Count;
         if (count == 0)
            return new None<Tuple<Block, Block>>();

         var last = count - 1;
         var invoke = block[last].As<Invoke>();
         if (invoke.IsSome)
         {
            var increment = invoke.Value.Arguments.ArgumentsBlock;
            var name = NameFromVariable(block[--last]);
            if (name.IsSome)
            {
               var builder = new CodeBuilder();
               for (var i = 0; i < last; i++)
                  builder.Verb(block[i]);
               builder.Variable(VAR_ACCUM);
               var invariant = builder.Block;
               return tuple(invariant, increment).Some();
            }
         }
         return new None<Tuple<Block, Block>>();
      }
   }
}