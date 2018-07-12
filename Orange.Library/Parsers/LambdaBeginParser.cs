using System;
using System.Collections.Generic;
using Orange.Library.Parsers.Special;
using Orange.Library.Values;
using Standard.Types.Collections;
using Standard.Types.Maybe;
using Standard.Types.RegularExpressions;
using Standard.Types.Tuples;
using static Orange.Library.OrangeCompiler;
using static Orange.Library.Parsers.DefineParser;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Parsers.Parser;
using static Orange.Library.Parsers.Special.ParametersParser;
using static Orange.Library.Runtime;
using static Standard.Types.Tuples.TupleFunctions;

namespace Orange.Library.Parsers
{
   public class LambdaBeginParser
   {
      Matcher matcher;

      public LambdaBeginParser()
      {
         matcher = new Matcher();
      }

      public IMaybe<Tuple<Parameters, Block, int>> Parse(string source, int index)
      {
         bool splatting;
         Block block;
         Parameters newParameters;
         int newIndex;
         if (retrieveBlock(source, index, "").Assign(out newIndex, out splatting, out block))
         {
            index = newIndex;
            var every = new EveryFillInBlock(block, new List<Parameter>());
            every.ClosureBlock(block).Assign(out block, out newParameters, out splatting);
            return tuple(newParameters, block, index).Some();
         }
         if (matcher.IsMatch(source.Substring(index), $"^ /(/s*) /(['(['] | {REGEX_VARIABLE})"))
         {
            Color(index, matcher[0, 1].Length, Whitespaces);
            var next = matcher[0, 2];
            index += matcher[0].Length;
            if (next == "(" || next == "[")
            {
               var parametersType = next == "[" ? ParametersType.Pattern : ParametersType.Standard;
               Color(next.Length, Structures);
               var parametersParser = new ParametersParser(parametersType);
               if (parametersParser.Parse(source, ref index))
                  newParameters = parametersParser.Parameters;
               else
                  return new None<Tuple<Parameters, Block, int>>();
            }
            else
            {
               Color(next.Length, Variables);
               var builder = new CodeBuilder();
               builder.Variable(next);
               if (next == "_")
               {
                  if (retrieveBlock(source, index, next).Assign(out newIndex, out splatting, out block))
                  {
                     index = newIndex;
                     var every = new EveryFillInBlock(block, new List<Parameter>());
                     every.ClosureBlock(block).Assign(out block, out newParameters, out splatting);
                     return tuple(newParameters, block, index).Some();
                  }
                  return new None<Tuple<Parameters, Block, int>>();
               }
               var parametersParser = new ParametersParser();
               if (parametersParser.Parse(builder.Block))
                  newParameters = parametersParser.Parameters;
               else
                  return new None<Tuple<Parameters, Block, int>>();
            }
            if (retrieveBlock(source, index, next).Assign(out newIndex, out splatting, out block))
            {
               index = newIndex;
               newParameters.Splatting = splatting;
               return tuple(newParameters, block, index).Some();
            }
         }
         return new None<Tuple<Parameters, Block, int>>();
      }

      IMaybe<Tuple<int, bool, Block>> retrieveBlock(string source, int index, string next)
      {
         if (matcher.IsMatch(source.Substring(index), "^ /s* /('->' | '=>') /s*"))
         {
            Reject(IsDefinedKeyword(next), "Lambda", $"{next} is a keyword");
            var bridgeLength = matcher[0].Length;
            Color(index, bridgeLength, Structures);
            index += bridgeLength;
            Block block;
            ParseBlock(source, index, "'}'").Assign(out block, out index);
            var splatting = matcher[0, 1] == "=>";
            return tuple(index, splatting, block).Some();
         }
         return new None<Tuple<int, bool, Block>>();
      }
   }
}