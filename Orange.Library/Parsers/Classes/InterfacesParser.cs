using System.Linq;
using Orange.Library.Classes;
using Orange.Library.Values;
using Orange.Library.Verbs;
using Standard.Types.Strings;

namespace Orange.Library.Parsers.Classes
{
	public class InterfacesParser : Parser, IClassParser
	{
		public InterfacesParser()
			: base(@"^(\s*\()([^)]+)(\))")
		{
		}

		public override Verb CreateVerb(string[] tokens)
		{
			Color(position, tokens[1].Length, IDEColor.EntityType.Structure);
			string interfaces = tokens[2];
			Color(interfaces.Length, IDEColor.EntityType.Variable);
			Color(1, IDEColor.EntityType.Structure);
			Builder.Mixins = interfaces.Split(@"\s+").Select(getMixin).ToArray();
			return new NullOp();
		}

		Mixin getMixin(string mixinSource)
		{
			var matcher = new Matcher();
			if (matcher.IsMatch(mixinSource, @"^(?:(" + Runtime.REGEX_VARIABLE + @")\s*->\s*)?(" + Runtime.REGEX_VARIABLE + @")(?:\.(" +
				Runtime.REGEX_VARIABLE + "))?$"))
			{
				string alias = matcher[0, 1];
				string className = matcher[0, 2];
				string message = matcher[0, 3];
				Mixin.MixinType type = message.IsEmpty() ? Mixin.MixinType.Interface : Mixin.MixinType.Message;
				return new Mixin
				{
					ClassName = className,
					Message = message,
					Alias = alias,
					Type = type
				};
			}
			return new Mixin
			{
				ClassName = mixinSource,
				Message = "",
				Alias = "",
				Type = Mixin.MixinType.Interface
			};
		}

		public override string VerboseName
		{
			get
			{
				return "required messages";
			}
		}

		public ClassBuilder Builder
		{
			get;
			set;
		}

		public bool EndOfClass
		{
			get
			{
				return false;
			}
		}

		public Class.VisibilityType Visibility
		{
			get;
			set;
		}

		public Class.ScopeType Scope
		{
			get;
			set;
		}
	}
}