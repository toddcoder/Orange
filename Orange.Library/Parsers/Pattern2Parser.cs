using System.Collections.Generic;
using System.Linq;
using Orange.Library.Parsers.Conditionals;
using Orange.Library.Parsers.Patterns;
using Orange.Library.Patterns2;
using Orange.Library.Values;
using Orange.Library.Verbs;
using Standard.Types.Objects;

namespace Orange.Library.Parsers
{
	public class Pattern2Parser : Parser
	{
		bool subPattern;

		public Pattern2Parser(bool subPattern = false)
			: base(subPattern ? "^ /s* '['" : "^ /s* '^('")
		{
			this.subPattern = subPattern;
		}

		public override Verb CreateVerb(string[] tokens)
		{
			Color(position, length, IDEColor.EntityType.Structure);
			var index = position + length;

			var parsers = new List<Parser>
			{
				new StringElementParser(),
				new FieldScanElementParser(),
				new ListElementParser(),
				new BalanceElementParser(),
				new LengthElementParser(),
				new FieldDelimiterElementParser(),
				new TabElementParser(),
				new RemElementParser(),
				new WordBoundaryElementParser(),
				new StringBoundaryElementParser(),
				new AtElementParser(),
				new AnyClassElementParser(),
				new AssertElementParser(),
				new BreakXElementParser(),
				new SpanElementParser(),
				new ArbNoElementParser(),
				new ArbElementParser(),
				new ClassElementParser(),
				new AlternateElementParser(),
				new OptionalElementParser(),
				new RangeElementParser(),
				new CountElementParser(),
				new Pattern2Parser(true),
				new AnyElementParser(),
				new BlockElementParser(),
				new SingleCharacterElementParser(),
				new HexElementParser(),
				new FunctionElementParser(),
				new VariableElementParser(),
				new EndBlockParser("]", true)
			};

			var instructions = new List<Instruction>();

			var scanning = true;
			var sourceLength = source.Length;
			var isAlternate = false;
			var isOptional = false;
			var not = false;
			var conditionalParser = new ConditionalParser();
			var replacementParser = new ReplacementParser();
			var found = false;

			while (scanning && index < sourceLength)
			{
				scanning = false;
				found = false;
				foreach (var parser in parsers.Where(parser => parser.Scan(source, index)))
				{
					EndBlockParser endBlockParser;
					if (parser.IsA(out endBlockParser))
					{
						//replacement = endBlockParser.Replacement;
						index = endBlockParser.Result.Position;
						found = true;
						break;
					}

					if (parser is AlternateElementParser)
					{
						isAlternate = true;
						index = parser.Result.Position;
						scanning = true;
						found = true;
						break;
					}

					if (parser is OptionalElementParser)
					{
						isOptional = true;
						index = parser.Result.Position;
						scanning = true;
						found = true;
						break;
					}

					IInstructionParser instructionParser;
					Runtime.Assert(parser.IsA(out instructionParser), "Pattern 2 parser", "Instruction not supported");

					var instruction = instructionParser.Instruction;
					var currentIndex = instructions.Count - 1;
					var currentInstruction = currentIndex == -1 ? instruction : instructions[currentIndex];

					if (not)
					{
						instruction.Not = true;
						not = false;
					}

					if (instruction is NegateInstruction)
					{
						not = true;
						index = parser.Result.Position;
						scanning = true;
						found = true;
						break;
					}

					instruction.ID = Compiler.State.ObjectID();
					index = parser.Result.Position;

					if (replacementParser.Scan(source, index))
					{
						instruction.Replacement = replacementParser.Replacement;
						index = replacementParser.Result.Position;
					}

					if (conditionalParser.Scan(source, index))
						index = conditionalParser.Result.Position;
					instruction.Conditional = conditionalParser.Conditional;

					if (isOptional)
					{
						instruction.Alternate = new StringInstruction("");
						isOptional = false;
					}

					if (isAlternate)
					{
						currentInstruction.AppendAlternate(instruction);
						isAlternate = false;
					}

					else
						instructions.Add(instruction);
					scanning = true;
					found = true;
					break;
				}
			}

			Runtime.Assert(found, "Pattern parser", "Didn't understand pattern '{0}'", source.Substring(index));

			if (instructions.Count == 0)
				instructions.Add(new FailedInstruction());

			var newPattern = new Pattern2(instructions, subPattern);
			overridePosition = index;
			newPattern.Source = source.Substring(position, index - position).Trim();
			/*			if (resultElement)
						{
							Element = new PatternElement(newPattern);
							return new NullOp();
						}*/
			result.Value = newPattern;
			return new Push(newPattern);
		}

		public override string VerboseName
		{
			get
			{
				return null;
			}
		}
	}
}