namespace Orange.Library.Classes
{
	public class Mixin
	{
		public enum MixinType
		{
			Interface,
			Message,
			Abstract
		}

		public string ClassName
		{
			get;
			set;
		}

		public string Message
		{
			get;
			set;
		}

		public string Alias
		{
			get;
			set;
		}

		public MixinType Type
		{
			get;
			set;
		}
	}
}