using System.Collections.Generic;
using Orange.Library.Patterns;
using Orange.Library.Values;
using Standard.Types.Collections;
using static Orange.Library.Managers.RegionManager;
using static Orange.Library.Runtime;

namespace Orange.Library.Managers
{
	public class PatternManager
	{
		const string LOCATION = "Pattern manager";

		public class PatternState
		{
			ConditionalReplacements replacements;
			bool anchored;
			Stack<AlternateData> alternates;
			Stack<string> workingInputStack;
			string workingInput;
			Hash<string, string> bindings;

			public PatternState()
			{
				replacements = new ConditionalReplacements();
				IgnoreCase = false;
				anchored = false;
				Position = 0;
				FirstScan = true;
				alternates = new Stack<AlternateData>();
				workingInputStack = new Stack<string>();
				Multi = false;
				workingInput = "";
				bindings = new Hash<string, string>();
			}

			public ConditionalReplacements Replacements => replacements;

		   public int Position
			{
				get;
				set;
			}

			public bool Anchored
			{
				get => anchored;
			   set
				{
					anchored = value;
					FirstScan = !anchored;
				}
			}

			public bool IgnoreCase
			{
				get;
				set;
			}

			public string Input
			{
				get;
				set;
			}

			public string CurrentInput => Input.Substring(Position);

		   public bool Aborted
			{
				get;
				set;
			}

			public bool FirstScan
			{
				get;
				set;
			}

			public Stack<AlternateData> Alternates => alternates;

		   public string WorkingInput
			{
				get => workingInput;
		      set => workingInput = value;
		   }

			public void SaveWorkingInput() => workingInputStack.Push(WorkingInput);

		   public void RestoreWorkingInput() => WorkingInput = workingInputStack.Pop();

		   public bool Trace
			{
				get;
				set;
			}

			public bool Multi
			{
				get;
				set;
			}

			public Value Result
			{
				get;
				set;
			}

			public void Bind(string name, string value) => bindings[name] = value;

		   public void AssignBindings()
			{
				foreach (var item in bindings)
					Regions.SetParameter(item.Key, item.Value);
			}
		}

		Stack<PatternState> patternStates;
		Depth depth;

		public PatternManager()
		{
			patternStates = new Stack<PatternState>();
			depth = new Depth(MAX_VAR_DEPTH, LOCATION);
		}

		public PatternState Current => patternStates.Peek();

	   public ConditionalReplacements Replacements => Current.Replacements;

	   public int Position
		{
			get => Current.Position;
	      set => Current.Position = value;
	   }

		public bool Anchored
		{
			get => Current.Anchored;
		   set => Current.Anchored = value;
		}

		public bool IgnoreCase
		{
			get => Current.IgnoreCase;
		   set => Current.IgnoreCase = value;
		}

		public string Input
		{
			get => Current.Input;
		   set => Current.Input = value;
		}

		public string CurrentInput => Current.CurrentInput;

	   public bool Aborted
		{
			get => Current.Aborted;
	      set => Current.Aborted = value;
	   }

		public bool FirstScan
		{
			get => Current.FirstScan;
		   set => Current.FirstScan = value;
		}

		public Stack<AlternateData> Alternates => Current.Alternates;

	   public string WorkingInput
		{
			get => Current.WorkingInput;
	      set => Current.WorkingInput = value;
	   }

		public void SaveWorkingInput() => Current.SaveWorkingInput();

	   public void RestoreWorkingInput() => Current.RestoreWorkingInput();

	   public bool Trace
		{
			get => Current.Trace;
	      set => Current.Trace = value;
	   }

		public bool Multi
		{
			get => Current.Multi;
		   set => Current.Multi = value;
		}

		public Value Result
		{
			get => Current.Result;
		   set => Current.Result = value;
		}

		public void Push()
		{
			patternStates.Push(new PatternState());
			Assert(patternStates.Count <= MAX_BLOCK_DEPTH, LOCATION, "Excessive recursion");
		}

		public void Pop() => patternStates.Pop();

	   public void PushDepth()
		{
			try
			{
				depth.Retain();
			}
			finally
			{
				depth.Reset();
			}
		}

		public void PopDepth()
		{
			try
			{
				depth.Release();
			}
			finally
			{
				depth.Reset();
			}
		}

		public int Depth => patternStates.Count;

	   public void Bind(string name, string value) => Current.Bind(name, value);

	   public void AssignBindings() => Current.AssignBindings();
	}
}