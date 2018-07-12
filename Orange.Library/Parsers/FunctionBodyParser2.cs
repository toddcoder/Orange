using Orange.Library.Values;
using Standard.Types.Objects;
using Standard.Types.Strings;

namespace Orange.Library.Parsers
{
	public class FunctionBodyParser2
	{
		const string LOCATION = "Function body parser";
		const string ERROR_MESSAGE = "Couldn't find block";

		static bool createCondition(Block block, out Block newBlock, out Block newCondition, out Block newWhere)
		{
			var blockBuilder = new CodeBuilder();
			var conditionBuilder = new CodeBuilder();
			var buildingCondition = false;
			newWhere = null;
			foreach (var verb in block.AsAdded)
			{
				if (buildingCondition)
				{
					conditionBuilder.Verb(verb);
					continue;
				}
				Verbs.If _if;
				Verbs.Where where;
				if (verb.IsA(out _if))
				{
					buildingCondition = true;
					continue;
				}
				if (verb.IsA(out where))
				{
					newWhere = where.Block;
					continue;
				}
				blockBuilder.Verb(verb);
			}
			if (buildingCondition)
			{
				newBlock = blockBuilder.Block;
				newCondition = conditionBuilder.Block;
			}
			else
				newBlock = newCondition = null;
			return buildingCondition;
		}

		Matcher matcher;
		BlockParser blockParser;

		public FunctionBodyParser2()
		{
			matcher = new Matcher();
			blockParser = new BlockParser(false);
		}

		public bool Parse(string source, ref int index, out Block condition, out Block where)
		{
			var input = source.Sub(index);
			if (matcher.IsMatch(input, @"^\s*{"))
			{
				Runtime.Assert(blockParser.Scan(source, index), LOCATION, ERROR_MESSAGE);
				var result = blockParser.Result;
				index = result.Position;
				Block = (Block)result.Value;
				MultiCapable = true;
				condition = null;
				where = null;
				return true;
			}
			if (matcher.IsMatch(input, @"^\s*=\s*"))
			{
				var length = matcher[0].Length;
				Parser.Color(index, length, IDEColor.EntityType.Structure);
				index += length;
				Block = OneLineBlockParser.Parse(source, ref index, false);
				MultiCapable = false;
				Block newBlock;
				if (createCondition(Block, out newBlock, out condition, out where))
					Block = newBlock;
				return true;
			}
			condition = null;
			where = null;
			return false;
		}

		public Block Block
		{
			get;
			set;
		}

		public bool MultiCapable
		{
			get;
			set;
		}
	}
}