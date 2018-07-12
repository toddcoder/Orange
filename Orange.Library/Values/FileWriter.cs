using System;
using System.IO;
using Orange.Library.Managers;

namespace Orange.Library.Values
{
	public class FileWriter : Value, IDisposable
	{
		StreamWriter writer;
		Library.Buffer buffer;
		bool flushed;
		bool closed;

		public FileWriter(StreamWriter writer)
		{
			this.writer = writer;
			buffer = new Library.Buffer();
			flushed = false;
			closed = false;
		}

		public override int Compare(Value value) => 0;

	   public override string Text
		{
			get
			{
				return buffer.ToString();
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

		public override ValueType Type => ValueType.FileWriter;

	   public override bool IsTrue => !closed;

	   public override Value Clone() => new FileWriter(writer);

	   protected override void registerMessages(MessageManager manager)
		{
			manager.RegisterProperty(this, "out", v => GetOut(),
            v => ((FileWriter)v).SetOut());
			manager.RegisterProperty(this, "rout", v => GetROut(),
				v => ((FileWriter)v).SetROut());
			manager.RegisterProperty(this, "put", v => GetPut(),
            v => ((FileWriter)v).SetPut());
			manager.RegisterProperty(this, "write", v => GetWrite(),
				v => ((FileWriter)v).SetWrite());
			manager.RegisterMessage(this, "close", v => ((FileWriter)v).Close());
			manager.RegisterMessage(this, "flush", v => ((FileWriter)v).Flush());
			manager.RegisterMessage(this, "open?", v => ((FileWriter)v).Open());
		}

		public Value Out() => new ValueAttributeVariable("out", this);

	   public static Value GetOut() => "";

	   public Value SetOut()
		{
			if (closed)
				return new Nil();
			flushed = false;
	      buffer.Print(Arguments[0].Text);
			return this;
		}

		public Value ROut() => new ValueAttributeVariable("rout", this);

	   public static Value GetROut() => "";

	   public Value SetROut()
		{
			if (closed)
				return new Nil();
			flushed = false;
	      buffer.Print(Arguments[0]);
			return this;
		}

		public Value Put() => new ValueAttributeVariable("put", this);

	   public static Value GetPut() => "";

	   public Value SetPut()
		{
			if (closed)
				return new Nil();
			flushed = false;
	      buffer.Put(Arguments[0].Text);
			return this;
		}

		public Value Write() => new ValueAttributeVariable("write", this);

	   public static Value GetWrite() => "";

	   public Value SetWrite()
		{
			if (closed)
				return new Nil();
			flushed = false;
	      buffer.Write(Arguments[0].Text);
			return this;
		}

		public Value Close()
		{
			Flush();
			writer.Close();
			closed = true;
			return this;
		}

		public Value Flush()
		{
			if (closed)
				return new Nil();
			if (!flushed)
			{
				writer.Write(buffer.Result());
				writer.Flush();
				flushed = true;
			}
			return this;
		}

		public void Dispose() => Close();

	   public Value Open() => !closed;
	}
}