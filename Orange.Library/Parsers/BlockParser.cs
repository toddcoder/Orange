using System;
using Orange.Library.Values;
using Orange.Library.Verbs;
using Standard.Types.Collections;
using Standard.Types.Maybe;
using static Orange.Library.OrangeCompiler;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Runtime;
using static Orange.Library.Verbs.Verb.AffinityType;

namespace Orange.Library.Parsers
{
	public class BlockParser : Parser
	{
		static Block modifyBlock(Block sourceBlock)
		{
			Block resultBlock;
			if (pushBlock(sourceBlock).Assign(out resultBlock))
				return resultBlock;

			return sourceBlock;
		}

		static IMaybe<Tuple<Block, Parameters>> lambdaFromBlock(Block sourceBlock) => fillInBlock(sourceBlock);

	   static IMaybe<Block> pushBlock(Block sourceBlock)
		{
			if (sourceBlock.Count != 1)
				return new None<Block>();

			var verb = sourceBlock[0];
			if (verb is Push || verb is PushArrayLiteral)
			{
				var resultBlock = sourceBlock;
				resultBlock.ImmediatelyResolvable = false;
			   return resultBlock.Some();
			}
			return new None<Block>();
		}

		static IMaybe<Tuple<Block, Parameters>> fillInBlock(Block sourceBlock)
		{
		   if (sourceBlock.Count == 0)
		      return new None<Tuple<Block, Parameters>>();
			var asAdded = sourceBlock.AsAdded;
		   if (isLeadingOperator(asAdded[0]) || isTrailingOperator(asAdded[asAdded.Count - 1]))
		      return FillInBlockParser.FillInBlock(sourceBlock).Some();
		   return new None<Tuple<Block, Parameters>>();
		}

		public static bool isLeadingOperator(Verb verb)
		{
			return verb.IsOperator && (verb.Affinity == Infix || verb.Affinity == Postfix);
		}

		public static bool isTrailingOperator(Verb verb)
		{
			return verb.IsOperator && (verb.Affinity == Infix || verb.Affinity == Prefix);
		}

		LambdaBeginParser lambdaBeginParser;
		bool allowLambda;

		public BlockParser(bool allowLambda)
			: base("^ [' ' /t]* /(['{('])")
		{
			lambdaBeginParser = new LambdaBeginParser();
			this.allowLambda = allowLambda;
		}

		public override Verb CreateVerb(string[] tokens)
		{
			Color(position, length, Structures);
			var index = position + length;
			Parameters parameters;
			Block block;
		   int newIndex;
			if (lambdaBeginParser.Parse(source, index).Assign(out parameters, out block, out newIndex))
			{
			   index = newIndex;
				RejectNull(parameters, "Block parser", "Parameters malformed");
				var lambda = new Lambda(new Region(), block, parameters, false)
				{
					Splatting = parameters.Splatting
				};
				result.Value = lambda;
				overridePosition = index;
				return new CreateLambda(parameters, block, parameters.Splatting);
			}
			ParseBlock(source, index, EndStructure(tokens[1])).Assign(out block, out index);
			block.ImmediatelyResolvable = tokens[0].EndsWith("(");
			block = modifyBlock(block);
			overridePosition = index;
			Block lambdaBlock;
			if (allowLambda && lambdaFromBlock(block).Assign(out lambdaBlock, out parameters))
			{
				result.Value = new Lambda(new Region(), lambdaBlock, parameters, false);
				return new CreateLambda(parameters, lambdaBlock, parameters.Splatting);
			}
			result.Value = block;
			return new Push(block);
		}

		public override string VerboseName => "block";
	}
}