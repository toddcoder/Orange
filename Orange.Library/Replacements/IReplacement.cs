using Standard.Types.Maybe;

namespace Orange.Library.Replacements
{
	public interface IReplacement
	{
		string Text
		{
			get;
		}

		bool Immediate
		{
			get;
			set;
		}

		long ID
		{
			get;
		}

		void Evaluate();

		IReplacement Clone();

		Arguments Arguments
		{
			get;
			set;
		}

	   IMaybe<long> FixedID
	   {
	      get;
	      set;
	   }
	}
}