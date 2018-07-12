using Orange.Library.Managers;

namespace Orange.Library.Values
{
	public class GeneratorComprehension : Value, IGenerator
	{
		Block block;
		GeneratorComprehension innerComprehension;
		Generator generator;
		Block ifBlock;
		Region region;

		public GeneratorComprehension(Block block, Region region = null)
		{
			this.block = block;
			if (region == null)
			{
				this.region = new Region();
				RegionManager.Regions.Current.CopyAllVariablesTo(this.region);
			}
			else
				this.region = region;
		}

		public GeneratorComprehension(GeneratorComprehension comprehension)
		{
			block = comprehension.block;
			generator = comprehension.generator;
			region = comprehension.region;
			if (comprehension.innerComprehension != null)
				innerComprehension = comprehension.innerComprehension;
		}

		public Generator Generator
		{
			get => generator;
		   set => generator = value;
		}

		public Block IfBlock
		{
			get => ifBlock;
		   set => ifBlock = value;
		}

		public override int Compare(Value value) => 0;

	   public override string Text
		{
			get
			{
				return "";
			}
			set
			{
			}
		}

		public override double Number
		{
			get;
			set;
		}

		public override ValueType Type => ValueType.String;

	   public override bool IsTrue => false;

	   public override Value Clone()
		{
			if (block != null)
			{
				var clonedBlock = (Block)block.Clone();
				return new GeneratorComprehension(clonedBlock, region.Clone())
				{
					Generator = (Generator)Generator.Clone(),
					ifBlock = (Block)ifBlock?.Clone()
				};
			}
			return new GeneratorComprehension((GeneratorComprehension)innerComprehension?.Clone())
			{
				Generator = (Generator)Generator.Clone(),
				ifBlock = (Block)ifBlock?.Clone()
			};
		}

		protected override void registerMessages(MessageManager manager)
		{
			manager.RegisterMessage(this, "if", v => ((GeneratorComprehension)v).If());
			manager.RegisterMessage(this, "from", v => ((GeneratorComprehension)v).Map());
			manager.RegisterMessage(this, "next", v => ((GeneratorComprehension)v).Next());
			manager.RegisterMessage(this, "reset", v => ((GeneratorComprehension)v).Reset());
		}

		public void Before()
		{
		}

		public Value Next(int index = 0)
		{
			using (var popper = new RegionPopper(region, "generator-comprehension"))
			{
				popper.Push();
				if (generator == null)
					return new Nil();
				if (ifBlock != null)
				{
					if (innerComprehension == null)
					{
						var value = generator.Next();
						if (value.IsNil)
							return value;
						region.SetParameter(generator.ParameterName, value);
						if (ifBlock.Evaluate().IsTrue)
						{
							value = block.Evaluate();
							if (!value.IsNil)
								return value;
						}
					}
					else
					{
						var value = generator.Next();
						if (value.IsNil)
							return value;
						region.SetParameter(generator.ParameterName, value);
						if (ifBlock.Evaluate().IsTrue)
							return innerComprehension.Next(index);
					}
				}
				else
				{
					if (innerComprehension == null)
					{
						var value = generator.Next();
						if (value.IsNil)
							return value;
						region.SetParameter(generator.ParameterName, value);
						value = block.Evaluate();
						return value;
					}
					else
					{
						var value = generator.Next();
						if (value.IsNil)
							return value;
						region.SetParameter(generator.ParameterName, value);
						return innerComprehension.Next(index);
					}
				}
				return null;
			}
		}

		public void End()
		{
		}

		void pushDownInnerComprehension(GeneratorComprehension comprehension)
		{
			if (innerComprehension != null)
				innerComprehension.pushDownInnerComprehension(comprehension);
			else
			{
				innerComprehension = comprehension;
				innerComprehension.block = block;
				block = null;
			}
		}

		public Value Map()
		{
			var comprehension = new GeneratorComprehension(block)
			{
				generator = (Generator)Arguments[0]
			};
			pushDownInnerComprehension(comprehension);
			block = null;
			return this;
		}

		public Value If()
		{
			setIf(Arguments.Block);
			return this;
		}

		void setIf(Block block)
		{
			if (innerComprehension == null)
				ifBlock = block;
			else
				innerComprehension.setIf(block);
		}

		public Value Reset()
		{
			generator.Reset();
		   innerComprehension?.Reset();
		   return this;
		}

		public override bool IsArray => true;

	   public override Value SourceArray => getArray();

	   public override Value AlternateValue(string message) => getArray();

	   Value getArray()
		{
			using (var popper = new RegionPopper(region, "generator-comprehension-get-array"))
			{
				popper.Push();
				var array = new Array();
				for (var i = 0; i < Runtime.MAX_ARRAY; i++)
				{
					var value = Next(i);
					if (value.IsNil)
						break;
					array.Add(value);
				}
				return array;
			}
		}
	}
}