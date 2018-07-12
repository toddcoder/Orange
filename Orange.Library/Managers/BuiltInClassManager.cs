using System;
using Orange.Library.Values;
using Orange.Library.Verbs;
using Standard.Types.Collections;
using Standard.Types.Objects;
using static System.Math;
using static Orange.Library.Runtime;

namespace Orange.Library.Managers
{
	public class BuiltInClassManager
	{
		public static BuiltInClassManager BuiltIns
		{
			get;
			set;
		}

		Hash<string, Func<ClassBuilder>> registeredClasses;
		Hash<string, bool> deployedClasses;

		public BuiltInClassManager()
		{
			registeredClasses = new Hash<string, Func<ClassBuilder>>();
			deployedClasses = new Hash<string, bool>();

/*			Register("Some", ClassSome);
			Register("None", ClassNone);*/
			//Register("Const", ClassConst);
			Register("Array", ClassArray);
		}

		public void Register(string className, Func<ClassBuilder> func)
		{
			registeredClasses[className] = func;
			deployedClasses[className] = false;
		}

		public bool IsRegistered(string className) => registeredClasses.ContainsKey(className);

	   public bool IsDeployed(string className) => deployedClasses[className];

	   public void DeployIfPossible(Value value)
	   {
	      var variable = value.As<Variable>();
			if (variable.IsSome && IsRegistered(variable.Value.Name))
				Deploy(variable.Value.Name);
		}

		public void Deploy(string className)
		{
			if (!IsRegistered(className) || IsDeployed(className))
				return;

			var builder = registeredClasses[className]();
		   builder?.Create();
		   deployedClasses[className] = true;
		}

		static ClassBuilder ClassSome()
		{
			var builder = new ClassBuilder("Some");
			builder.Parameter("value");
			builder.Parameters();
			return builder;
		}

		static ClassBuilder ClassNone() => new ClassBuilder("None");

	   static ClassBuilder ClassConst()
		{
			var builder = new ClassBuilder("Const");

			builder.BeginClassBlock();

			var code = builder.Builder;

			code.ReturnValueFunction("tab", STRING_TAB);
			code.ReturnValueFunction("crlf", STRING_CRLF);
			code.ReturnValueFunction("cr", STRING_CR);
			code.ReturnValueFunction("lf", STRING_LF);
			code.ReturnValueFunction("letters", STRING_LETTERS);
			code.ReturnValueFunction("upper", STRING_UPPER);
			code.ReturnValueFunction("lower", STRING_LOWER);
			code.ReturnValueFunction("vowels", STRING_VOWELS);
			code.ReturnValueFunction("uvowels", STRING_UVOWELS);
			code.ReturnValueFunction("lvowels", STRING_LVOWELS);
			code.ReturnValueFunction("cons", STRING_LCONSONANTS + STRING_UCONSONANTS);
			code.ReturnValueFunction("ucons", STRING_UCONSONANTS);
			code.ReturnValueFunction("lcons", STRING_LCONSONANTS);
			code.ReturnValueFunction("digits", STRING_DIGITS);
			code.ReturnValueFunction("pi", PI);
			code.ReturnValueFunction("e", Exp(1));
			code.ReturnValueFunction("punct", STRING_PUNCT);
			code.ReturnValueFunction("words", STRING_WORDS);
			code.ReturnValueFunction("spaces", STRING_SPACES);
			code.ReturnValueFunction("quotes", STRING_QUOTES);
			code.ReturnValueFunction("degToRad", PI / 180);
			code.ReturnValueFunction("recPtn", (Pattern)(STRING_BEGIN_PATTERN + "'\r\n' | '\r' | '\n'" + STRING_END_PATTERN));
			code.ReturnValueFunction("recSep", "\r\n");
			code.ReturnValueFunction("fldPtn", (Pattern)(STRING_BEGIN_PATTERN + "+" + STRING_END_PATTERN));
			code.ReturnValueFunction("fldSep", " ");

			builder.EndClassBlock();

			return builder;
		}

		static ClassBuilder ClassArray()
		{
			var builder = new ClassBuilder("Array");
			builder.BeginClassBlock();

			var code = new CodeBuilder();

			code.Return();
			code.Variable("count");
			code.SendMessage("range");
			code.Verb(new Map());
			code.Push();
			code.Variable("value");
			code.PopAndPush();
			code.End();
			code.Parameter("count");
			code.Parameter("value");

			var lambda = code.Lambda();
			builder.Builder.Function("count_value_", lambda);

			builder.EndClassBlock();
			return builder;
		}
	}
}