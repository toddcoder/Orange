using Orange.Library.Managers;
using Standard.Types.Strings;

namespace Orange.Library.Values
{
	public class Word : Value
	{
		string word;
		int index;
		int position;
		string before;
		string after;
		Word next;
		Word previous;

		public Word(string word, int index, int position, string before = "", string after = "")
		{
			this.word = word;
			this.index = index;
			this.position = position;
			this.before = before;
			this.after = after;
			next = null;
			previous = null;
		}

		public override int Compare(Value value) => 0;

	   public override string Text
		{
			get
			{
				return before + word + after;
			}
			set
			{
			}
		}

		public override double Number
		{
			get
			{
				return Text.ToDouble();
			}
			set
			{
			}
		}

		public override ValueType Type => ValueType.Word;

	   public override bool IsTrue => Text.IsNotEmpty();

	   public override Value Clone() => new Word(word, index, position, before, after);

	   protected override void registerMessages(MessageManager manager)
		{
			manager.RegisterMessage(this, "word", v => ((Word)v).word);
			manager.RegisterMessage(this, "bef", v => ((Word)v).before);
			manager.RegisterMessage(this, "aft", v => ((Word)v).after);
			manager.RegisterMessage(this, "idx", v => ((Word)v).index);
			manager.RegisterMessage(this, "len", v => ((Word)v).word.Length);
			manager.RegisterMessage(this, "next", v => ((Word)v).Next());
			manager.RegisterMessage(this, "prev", v => ((Word)v).Previous());
			manager.RegisterMessage(this, "pos", v => ((Word)v).position);
		}

		public override Value AlternateValue(string message) => Text;

	   public override string ToString() => $"[{before}~{word}~{after}@{index}]";

	   public Value Next() => next ?? (Value)new Nil();

	   public void SetNext(Value nextWord) => next = (Word)nextWord;

	   public Value Previous() => previous ?? (Value)new Nil();

	   public void SetPrevious(Value previousWord) => previous = (Word)previousWord;
	}
}