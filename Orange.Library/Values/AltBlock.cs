using System;
using System.Collections.Generic;
using System.Linq;
using Orange.Library.Managers;
using Orange.Library.Verbs;
using Standard.Types.Objects;

namespace Orange.Library.Values
{
	public class AltBlock : Block
	{
		const string LOCATION = "Alternate block";

		static Verb changeToAltBlock(Verb verb)
		{
			Push push;
			if (verb.IsA(out push))
			{
				Block block;
				if (push.Value.IsA(out block))
				{
					if (!(block is AltBlock))
					{
						var value = block.AltBlock();
						return new Push(value);
					}
				}
			}
			CreateClosure createClosure;
			if (verb.IsA(out createClosure))
			{
				var parameters = createClosure.Parameters;
				var block = createClosure.Block;
				var splatting = createClosure.Splatting;
				block = (Block)block.AltBlock();
				return new CreateClosure(parameters, block, splatting);
			}

			return verb;
		}

		static void arrange(ref List<Verb> verbs)
		{
			var verbStack = new VerbStack();
			var newVerbs = new List<Verb>();

			foreach (var verb in verbs.Select(changeToAltBlock))
			{
				IEnd end;
				if (verb.IsA(out end))
				{
					while (!verbStack.IsEmpty)
					{
						var pendingVerb = verbStack.Pop();
						newVerbs.Add(pendingVerb);
					}
					continue;
				}
				while (verbStack.PendingReady(verb))
				{
					var pending = verbStack.Pop();
					newVerbs.Add(pending);
				}
				if (verb.Presidence == ExpressionManager.VerbPresidenceType.Push)
					newVerbs.Add(verb);
				else
					verbStack.Push(verb);
			}

			while (!verbStack.IsEmpty)
			{
				var verb = verbStack.Pop();
				newVerbs.Add(verb);
			}

			verbs = newVerbs;
		}

		public AltBlock(List<Verb> verbs)
			: base(verbs)
		{
			arrange(ref this.verbs);
		}

		public override Value Evaluate()
		{
			if (AutoRegister)
				Runtime.State.RegisterBlock(this, ResolveVariables);
			var valueStack = Runtime.State.Stack;
			IStringify stringify;
			var verbList = new Lazy<List<string>>(() => new List<string>());
			var watchList = new Lazy<List<string>>(getWatchList);

			foreach (var verb in verbs.TakeWhile(verb => !Runtime.State.ExitSignal && !Runtime.State.SkipSignal))
			{
/*				IEnd end;
				if (verb.IsA(out end))
				{
					value = "";
					if (end.EvaluateFirst)
						value = verb.Evaluate();
					if (end.IsEnd)
					{
						if (Runtime.State.DefaultParameterNames.PopAtEnd)
							Runtime.State.ClearDefaultParameterNames();
						valueStack.Clear();
					}
					else
						valueStack.Push(value);
					continue;
				}*/
				traceBefore(verbList, verb);
				var value = verb.Evaluate();
				if (value == null)
				{
					traceAfter(watchList);
					continue;
				}
				var block = value.AsBlock;
				if (block != null && block.ImmediatelyResolvable)
				{
					block.ReturnNull = false;
					value = block.Evaluate();
				}
				if (value != null)
					valueStack.Push(value);
				traceAfter(watchList);

				if (!Runtime.State.ReturnSignal)
					continue;

				var returnValue = Runtime.State.ReturnValue.Resolve();
				if (returnValue.IsA(out stringify))
					returnValue = stringify.String;
				shareNamespace(returnValue);
				if (AutoRegister)
					Runtime.State.UnregisterBlock();
				traceAfter(watchList, returnValue);
				return ReturnNull ? returnValue : (returnValue ?? "");
			}

			var result = valueStack.IsEmpty ? null : valueStack.Pop(ResolveVariables, LOCATION).ArgumentValue();
			if (result.IsA(out stringify))
				result = stringify.String;
			shareNamespace(result);
			if (AutoRegister)
				Runtime.State.UnregisterBlock();
			traceAfter(watchList, result);
			return ReturnNull ? result : (result ?? "");
		}

		public override string ToString()
		{
			return "-" + base.ToString() + "-";
		}
	}
}