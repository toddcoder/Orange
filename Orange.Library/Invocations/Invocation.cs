using System;
using System.Linq;
using System.Reflection;
using Orange.Library.Values;
using Standard.Types.Collections;
using Standard.Types.Numbers;
using Standard.Types.RegularExpressions;
using Standard.Types.Strings;
using static System.Reflection.BindingFlags;
using static Orange.Library.Invocations.Invocation.InvocationType;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Parsers.Parser;
using static Orange.Library.Runtime;

namespace Orange.Library.Invocations
{
	public class Invocation
	{
		public enum InvocationType
		{
			Instance,
			Static,
			New
		}

		public enum InvocationBindingType
		{
			Method,
			Property,
			Field
		}

		const string LOCATION = "Invocation";

		static Hash<string, Assembly> assemblies;
		static Hash<string, string> typeAliases;
		static Hash<string, string> typeToAssemblies;

		static Invocation()
		{
		   assemblies = new Hash<string, Assembly>();
		   typeAliases = new Hash<string, string>();
			typeToAssemblies = new Hash<string, string>();
		}

		InvocationType invocationType;
		Assembly assembly;
		Type type;
		string member;
		string[] parameters;
		Bits32<BindingFlags> bindingFlags;
		Matcher matcher;
		string source;
		int index;

		public Invocation(string source, int index)
		{
			this.source = source;
			this.index = index;
			initialize();
		}

		void initialize()
		{
			matcher = new Matcher();
		   matcher.RequiredMatch(source, $"^ ('|' /(-['|']+) '|')? (/({REGEX_VARIABLE}) /(/s* '=' /s*))? /('#'? [/w .]+) " +
		      "/(['.:']) /(/w+) '(' /(-[')']*) ')' /('.'? ['!?'])? $", $"Didn't understand invocation <[{source}]>", true);
		   matcher.Extract(0, out var assemblyName, out var alias, out var sign, out var typeName, out var dot, out member,
		         out var parameterSource, out var invocationMethod);
		   var assemblyPresent = assemblyName.IsNotEmpty();
			if (assemblyPresent)
			{
				Color(index, 1, Structures);
				Color(assemblyName.Length, Variables);
				Color(1, Structures);
				if (alias.IsNotEmpty())
				{
					Color(alias.Length, Variables);
					Color(sign.Length, Operators);
				}
				Color(typeName.Length, Variables);
			}
			else if (alias.IsNotEmpty())
			{
				Color(index, alias.Length, Variables);
				Color(sign.Length, Operators);
				Color(typeName.Length, Variables);
			}
			else
				Color(index, typeName.Length, Variables);
			Color(1, Structures);
			Color(member.Length, Messaging);
			Color(1, Structures);
			Color(parameterSource.Length, Variables);
			Color(1, Structures);
			Color(invocationMethod.Length, Operators);
			if (!assemblyPresent)
				assemblyName = "mscorlib";
			switch (member)
			{
				case "new":
					invocationType = New;
					bindingFlags = new Bits32<BindingFlags>(Public);
					break;
				default:
					invocationType = dot == "." ? InvocationType.Instance : InvocationType.Static;
					switch (invocationType)
					{
						case InvocationType.Instance:
							bindingFlags = BindingFlags.Instance;
							break;
						case InvocationType.Static:
							bindingFlags = BindingFlags.Static;
							break;
					}
					bindingFlags[Public] = true;
					switch (invocationMethod)
					{
						case "!":
							bindingFlags[SetProperty] = true;
							break;
						case "?":
							bindingFlags[GetProperty] = true;
							break;
						case ".!":
							bindingFlags[SetField] = true;
							break;
						case ".?":
							bindingFlags[GetField] = true;
							break;
						default:
							bindingFlags[InvokeMethod] = true;
							break;
					}
					break;
			}
		   if (typeName.StartsWith("#"))
		      typeName = typeAliases.Find(typeName.Skip(1), t => "");
			if (alias.IsNotEmpty())
				typeAliases[alias] = typeName;
			Assert(typeName.IsNotEmpty(), LOCATION, "Couldn't find type");
			var fullAssemblyName = typeToAssemblies[typeName];
			if (fullAssemblyName != null)
				assemblyName = fullAssemblyName;
			assembly = assemblies.Find(assemblyName,Assembly.Load);
			type = assembly.GetType(typeName, false, true);
			RejectNull(type, LOCATION, $"Didn't understand .NET type {typeName}");
			typeToAssemblies[typeName] = assemblyName;
			parameters = parameterSource.Split("/s* ',' /s*");
		}

		public Invocation()
		{
			source = "";
			index = -1;
		}

		public object Invoke(object target, Value[] arguments)
		{
			var args = parameters[0].IsEmpty() ? new object[0] : getArguments(arguments);
			switch (invocationType)
			{
				case InvocationType.Instance:
					RejectNull(target, LOCATION, "Target object is null");
					return target.GetType().InvokeMember(member, bindingFlags, null, target, args);
				case InvocationType.Static:
					return type.InvokeMember(member, bindingFlags, null, null, args);
				case New:
					return Activator.CreateInstance(type, args);
				default:
					return null;
			}
		}

		object[] getArguments(Value[] arguments) => parameters.Select((p, i) => getArgument(p, arguments[i])).ToArray();

	   object getArgument(string argumentType, Value value)
		{
			var isArray = matcher.IsMatch(argumentType, "/(.+) /('[]') $");
			string typeName;
		   if (isArray)
				matcher.Extract(0, out typeName, out _);
			else
				typeName = argumentType;
			return value.ToType(typeName, isArray);
		}

		public InvocationType Type => invocationType;
	}
}