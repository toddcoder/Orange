using System;
using System.Collections.Generic;
using System.Linq;
using Orange.Library.Parsers.Classes;
using Orange.Library.Values;
using Standard.Configurations;
using Standard.Types.Collections;
using Standard.Types.Objects;
using Standard.Types.Strings;
using Array = Orange.Library.Values.Array;

namespace Orange.Library.Classes
{
	public class ClassBuilder : ISerializeToGraph
	{
		const string LOCATION = "Class Builder";

		string superClassName;
		List<string> variables;
		Hash<string, Value> initializedVariables;
		Hash<string, Closure> methods;
		Hash<string, MultiClosure> multiMethods;
		Hash<string, Class.ScopeType> scope;
		Hash<string, Class.VisibilityType> objectVisibility;
		Hash<string, Class.VisibilityType> classVisibility;
		Class.ScopeType currentScope;
		Stack<Class.VisibilityType> currentVisiblity;
		Hash<string, string> delegates;
		Hash<string, bool> abstractMessages;
		Hash<string, Block> variableCases;

		public ClassBuilder(string superClassName)
		{
			this.superClassName = superClassName;
			variables = new List<string>();
			initializedVariables = new Hash<string, Value>();
			methods = new Hash<string, Closure>();
			multiMethods = new Hash<string, MultiClosure>
			{
				NewValue = m => new MultiClosure(),
				AutoAddDefault = true
			};
			scope = new Hash<string, Class.ScopeType>
			{
				DefaultValue = Class.ScopeType.Object
			};
			objectVisibility = new Hash<string, Class.VisibilityType>
			{
				DefaultValue = Class.VisibilityType.Public
			};
			classVisibility = new Hash<string, Class.VisibilityType>
			{
				DefaultValue = Class.VisibilityType.Public
			};
			currentScope = Class.ScopeType.Object;
			currentVisiblity = new Stack<Class.VisibilityType>();
			currentVisiblity.Push(Class.VisibilityType.Public);
			delegates = new Hash<string, string>();
			abstractMessages = new Hash<string, bool>();
			variableCases = new Hash<string, Block>();
		}

		public ClassBuilder()
			: this("")
		{
		}

		public Mixin[] Mixins
		{
			get;
			set;
		}

		public void PushVisibility(Class.VisibilityType type)
		{
			currentVisiblity.Push(type);
		}

		public void PopVisibility()
		{
			currentVisiblity.Pop();
		}

		public void ToggleScope()
		{
			currentScope = currentScope == Class.ScopeType.Object ? Class.ScopeType.Class : Class.ScopeType.Object;
		}

		public void SetToClassScope()
		{
			currentScope = Class.ScopeType.Class;
		}

		public void SetToObjectScope()
		{
			currentScope = Class.ScopeType.Object;
		}

		public Class.VisibilityType CurrentVisiblity
		{
			get
			{
				return currentVisiblity.Count == 0 ? Class.VisibilityType.Public : currentVisiblity.Peek();
			}
		}

		void setScopeAndVisibility(string messageName, IClassParser parser)
		{
			if (parser == null)
			{
				scope[messageName] = Class.ScopeType.Object;
				objectVisibility[messageName] = Class.VisibilityType.Public;
				return;
			}
			scope[messageName] = parser.Scope;
			if (parser.Scope == Class.ScopeType.Object)
				objectVisibility[messageName] = parser.Visibility;
			else
				classVisibility[messageName] = parser.Visibility;
		}

		public void AddMethod(string messageName, Closure closure, IClassParser parser)
		{
			methods[messageName] = closure;
			setScopeAndVisibility(messageName, parser);
		}

		public void AddMultiMethod(string messageName, Closure closure, Block block, IClassParser parser)
		{
			multiMethods[messageName].Add(new MultiClosureItem(closure, block, false));
			setScopeAndVisibility(messageName, parser);
		}

		public void AddVariable(string messageName, IClassParser parser)
		{
			variables.Add(messageName);
			setScopeAndVisibility(messageName, parser);
		}

		public void AddInitializedVariable(string messageName, Value value, IClassParser parser, Block block)
		{
			initializedVariables[messageName] = value;
			setScopeAndVisibility(messageName, parser);
			if (block != null)
				variableCases[messageName] = block;
		}

		public void AddDelegate(string messageName, string messageDelegatedTo, IClassParser parser)
		{
			delegates[messageName] = messageDelegatedTo;
			setScopeAndVisibility(messageName, parser);
		}

		public void AddAbstractMessage(string messageName, IClassParser parser)
		{
			abstractMessages[messageName] = true;
			setScopeAndVisibility(messageName, parser);
		}

		public void AddVariableCase(string name, Block block)
		{
			variableCases[name] = block;
		}

		public Class Build(string className)
		{
			Class super;
			Runtime.Assert(Runtime.State[superClassName].IsA(out super), LOCATION, "Couldn't find super class {0}", superClassName);
			var cls = (Class)super.Clone(className);
			cls.Super = super;
			if (super.ContainsClassMethod("inherited"))
			{
				bool handled;
				super.Send(super, "inherited", new Arguments(cls), out handled);
			}
			foreach (string messageName in variables)
			{
				cls.CurrentScope = scope[messageName];
				cls.CurrentVisibility = currentVisibility(cls, messageName);
				cls.CreateVariable(messageName);
			}
			foreach (KeyValuePair<string, Value> item in initializedVariables)
			{
				cls.CurrentScope = scope[item.Key];
				cls.CurrentVisibility = currentVisibility(cls, item.Key);
				Value value = item.Value;
				var block = value as Block;
				Runtime.AssertIsNotNull(value, LOCATION, "value {0} not initialized properly", item.Key);
				switch (value.Type)
				{
					case Value.ValueType.Block:
						if (cls.CurrentScope == Class.ScopeType.Object)
							value = block.Evaluate();
						break;
					case Value.ValueType.ClassBuilder:
						ClassBuilder innerBuilder = ((ClassBuilderValue)value).Builder;
						if (innerBuilder.AutoInstantiate)
						{
							Class innerClass = innerBuilder.Build(Runtime.VAR_ANONYMOUS + Compiler.State.ObjectID());
							innerClass.Arguments = new Arguments();
							value = innerClass.New();
						}
						break;
				}
				cls.CreateVariable(item.Key, value);
			}
			var matcher = new Matcher();
			foreach (KeyValuePair<string, Closure> item in methods)
			{
				string messageName = item.Key;
				Closure closure = item.Value;
				cls.CurrentScope = scope[messageName];
				cls.CurrentVisibility = cls.CurrentScope == Class.ScopeType.Object ? objectVisibility[messageName] :
					classVisibility[messageName];
				if (matcher.IsMatch(messageName, Runtime.REGEX_ATTRIBUTE_MESSAGE, true))
				{
					string type = matcher[0, 1];
					string attributeName = matcher[0, 2];
					cls.CreateAttribute(attributeName, closure.Parameters, closure.Block, type);
				}
				else
					cls.CreateMethod(messageName, closure.Parameters, closure.Block);
			}
			foreach (KeyValuePair<string, MultiClosure> item in multiMethods)
			{
				string messageName = item.Key;
				MultiClosure multiClosure = item.Value;
				cls.CurrentScope = scope[messageName];
				cls.CurrentVisibility = cls.CurrentScope == Class.ScopeType.Object ? objectVisibility[messageName] :
					classVisibility[messageName];
				cls.CreateMultiMethod(messageName, multiClosure);
			}
			foreach (KeyValuePair<string, string> item in delegates)
			{
				string messageName = item.Key;
				string messageDelegatedTo = item.Value;
				cls.CurrentScope = scope[messageName];
				cls.CreateDelegate(messageName, messageDelegatedTo);
			}
			foreach (KeyValuePair<string, bool> item in abstractMessages)
			{
				string messageName = item.Key;
				cls.CurrentScope = scope[messageName];
				cls.CreateAbstract(messageName);
			}
			foreach (KeyValuePair<string, Block> item in variableCases)
			{
				string messageName = item.Key;
				cls.AddVariableCase(messageName, item.Value);
			}
			Class.InstantiateClass(cls);
			if (Mixins != null && Mixins.Length > 0)
			{
				var missing = new Hash<string, string>();
				foreach (Mixin mixin in Mixins)
					switch (mixin.Type)
					{
						case Mixin.MixinType.Interface:
							addInterface(missing, cls, mixin.ClassName);
							break;
						case Mixin.MixinType.Message:
							addMessage(cls, mixin.ClassName, mixin.Message, mixin.Alias);
							break;
					}
				Runtime.Assert(missing.Count == 0, LOCATION, "Messages {0} required", interfaceMessageList(missing));
			}
			return cls;
		}

		void addInterface(Hash<string, string> missing, Class cls, string interf)
		{
			Value value = Runtime.State[interf];
			if (value.Type == Value.ValueType.Class)
			{
				var interfaceClass = (Class)value;
				if (interfaceClass.IsAbstract)
				{
					checkAbstract(interfaceClass, cls);
					return;
				}
				findMissing(missing, interfaceClass.ObjectVariableMessages, cls.ContainsObjectVariable, interf);
				findMissing(missing, interfaceClass.ObjectMethodMessages, cls.ContainsObjectMethod, interf);
				findMissing(missing, interfaceClass.ObjectAttributeMessages, cls.ContainsObjectAttribute, interf);
				findMissing(missing, interfaceClass.ObjectDelegateMessages, cls.ContainsObjectDelegate, interf);
				findMissing(missing, interfaceClass.ClassVariableMessages, cls.ContainsClassVariable, interf);
				findMissing(missing, interfaceClass.ClassMethodMessages, cls.ContainsClassMethod, interf);
				findMissing(missing, interfaceClass.ClassAttributeMessages, cls.ContainsClassAttribute, interf);
				findMissing(missing, interfaceClass.ClassDelegateMessages, cls.ContainsClassDelegate, interf);
			}
			else if (value.IsArray)
			{
				var array = (Array)value.SourceArray;
				findMissing(missing, array.Values.Select(v => v.Text).ToArray(), m => !cls.ObjectRespondsTo(m) && !cls.RespondsTo(m),
					interf);
			}
			else
				Runtime.Assert(false, LOCATION, "Didn't understand {0}", interf);
		}

		void addMessage(Class target, string className, string message, string alias)
		{
			Value value = Runtime.State[className];
			Runtime.Assert(value.Type == Value.ValueType.Class, LOCATION, "{0} isn't a class", className);
			var source = (Class)value;
			string targetMessage = alias.HasContent() ? alias : message;
			if (source.ContainsClassMethod(message))
			{
				Closure closure = source.ClassMethods[message];
				target.CurrentScope = Class.ScopeType.Class;
				target.CreateMethod(targetMessage, closure.Parameters, closure.Block);
			}
			else if (source.ContainsClassVariable(message))
			{
				target.CurrentScope = Class.ScopeType.Class;
				target.CreateVariable(targetMessage);
			}
			else if (source.ContainsClassDelegate(message))
			{
				target.CurrentScope = Class.ScopeType.Class;
				target.CreateDelegate(message, targetMessage);
			}
			else if (source.ContainsObjectMethod(message))
			{
				Closure closure = source.ObjectMethods[message];
				target.CurrentScope = Class.ScopeType.Object;
				target.CreateMethod(targetMessage, closure.Parameters, closure.Block);
			}
			else if (source.ContainsObjectVariable(message))
			{
				target.CurrentScope = Class.ScopeType.Object;
				target.CreateVariable(targetMessage);
			}
			else if (source.ContainsObjectDelegate(message))
			{
				target.CurrentScope = Class.ScopeType.Object;
				target.CreateDelegate(message, targetMessage);
			}
		}

		void checkAbstract(Class abstractClass, Class currentClass)
		{
			string className = abstractClass.ClassName;
			Runtime.AssertIsNotNull(abstractClass, LOCATION, "{0} is not a class", className);
			Runtime.Assert(abstractClass.IsAbstract, LOCATION, "{0} is not an abstract class", className);
			Runtime.Assert(abstractClass.AbstractImplemented(currentClass), LOCATION, "{0} doesn't implement {1}", currentClass.ClassName,
				className);
		}

		Class.VisibilityType currentVisibility(Class cls, string messageName)
		{
			return cls.CurrentScope == Class.ScopeType.Object ? objectVisibility[messageName] : classVisibility[messageName];
		}

		string interfaceMessageList(Hash<string, string> missing)
		{
			var reverse = new Hash<string, string>
			{
				DefaultValue = ""
			};
			foreach (KeyValuePair<string, string> item in missing)
			{
				string interf = item.Value;
				string message = item.Key;
				string list = reverse[interf];
				if (list.IsEmpty())
					reverse[interf] = message;
				else
					reverse[interf] += ", " + message;
			}
			return reverse.Select(i => string.Format("{0}<{1}>", i.Key, i.Value)).Listify();
		}

		void findMissing(Hash<string, string> missing, string[] messages, Func<string, bool> func, string interfaceName)
		{
			foreach (string message in messages.Where(m => !func(m)))
				missing[message] = interfaceName;
		}

		public bool AutoInstantiate
		{
			get;
			set;
		}

		public ObjectGraph ToGraph(string name)
		{
			var graph = new ObjectGraph(name, subName: "ClassBuilder");

			graph["super"] = new ObjectGraph("super", superClassName);
			graph["vars"] = variables.EnumToGraph("vars");
			graph["init-vars"] = initializedVariables.ToGraph("init-vars");
			graph["methods"] = methods.ToGraph("methods", (k, v) => v.ToGraph("methods"));
			graph["multi-methods"] = multiMethods.ToGraph("multi-methods", (k, v) => v.ToGraph(k));
			graph["scope"] = scope.ToGraph("scope");
			graph["obj-visibility"] = objectVisibility.ToGraph("obj-visibility");
			graph["cls-visibility"] = classVisibility.ToGraph("cls-visibility");
			graph["delegates"] = delegates.ToGraph("delegates");
			graph["abs-msgs"] = abstractMessages.ToGraph("abs-msgs");
			graph["var-cases"] = variableCases.ToGraph("var-cases", (k, v) => v.ToGraph(k));

			return graph;
		}

		public void FromGraph(ObjectGraph graph)
		{
			superClassName = graph["super"].Value;
			variables = graph["vars"].EnumFromGraph(g => g.Value).ToList();
			initializedVariables = graph["init-vars"].FromGraph();
			methods = graph["methods"].FromGraph(g => (Closure)g.AsValue());
			multiMethods = graph["multi-methods"].FromGraph(g => (MultiClosure)g.AsValue());
			scope = graph["scope"].FromGraph(g => g.Value.ToEnumeration<Class.ScopeType>());
			objectVisibility = graph["obj-visibility"].FromGraph(g => g.Value.ToEnumeration<Class.VisibilityType>());
			classVisibility = graph["cls-visibility"].FromGraph(g => g.Value.ToEnumeration<Class.VisibilityType>());
			delegates = graph["delegates"].FromGraph(g => g.Value);
			abstractMessages = graph["abs-msgs"].FromGraph(g => g.Value == Runtime.STRING_TRUE);
		}
	}
}