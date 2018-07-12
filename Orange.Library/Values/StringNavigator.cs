using System;
using Orange.Library.Managers;
using Standard.Types.Strings;

namespace Orange.Library.Values
{
   public class StringNavigator : Value
   {
      public enum StatusType
      {
         Starting,
         Success,
         More,
         Fail
      }

      Slicer text;
      int position;
      int length;
      StatusType status;

      public StringNavigator(string text)
      {
         this.text = text;
         position = 0;
         length = 0;
         status = StatusType.Starting;
      }

      string getText() => text[position, length];

      public override int Compare(Value value) => getText().CompareTo(value.Text);

      public override string Text
      {
         get => getText();
         set
         {
            text[position, length] = value;
            text.Reset();
         }
      }

      public override double Number
      {
         get => Text.ToDouble();
         set => text = value.ToString();
      }

      public override ValueType Type => ValueType.StringNavigator;

      public override bool IsTrue => status == StatusType.Success;

      public override Value Clone() => new StringNavigator(text.ToString())
      {
         position = position,
         length = length
      };

      void adjust()
      {
         /*			if (!success)
						{
							if (more)
							{
								position = 0;
								length = 0;
								more = false;
							}
							return;
						}*/
         if (position < 0)
            position = 0;
         var textLength = text.Length;
         if (position >= textLength)
            position = textLength - 1;
         if (length < 0)
            length = 0;
         if (length > textLength)
            length = textLength;
         if (position + length > textLength)
            length = textLength - position;
      }

      void setAbsolutePosition(int newPosition)
      {
         position = newPosition;
         adjust();
      }

      void setRelativePosition(int amount)
      {
         position += amount;
         adjust();
      }

      void setAbsoluteLength(int newLength)
      {
         length = newLength;
         adjust();
      }

      void setRelativeLength(int amount)
      {
         length += amount;
         adjust();
      }

      string textAtPosition() => text.Substring(position);

      int startPosition() => position + length;

      protected override void registerMessages(MessageManager manager)
      {
         manager.RegisterMessage(this, "success?", v => v.IsTrue);
         manager.RegisterMessage(this, "position", v => ((StringNavigator)v).position);
         manager.RegisterMessage(this, "length", v => ((StringNavigator)v).length);
         manager.RegisterMessage(this, "to", v => ((StringNavigator)v).To());
         manager.RegisterMessage(this, "match", v => ((StringNavigator)v).Match());
         manager.RegisterMessage(this, "move", v => ((StringNavigator)v).Move());
         manager.RegisterMessage(this, "open", v => ((StringNavigator)v).Open());
         manager.RegisterMessage(this, "slide", v => ((StringNavigator)v).Slide());
         manager.RegisterMessage(this, "right", v => ((StringNavigator)v).Right());
         manager.RegisterMessage(this, "left", v => ((StringNavigator)v).Left());
         manager.RegisterMessage(this, "more?", v => ((StringNavigator)v).More());
         manager.RegisterMessage(this, "reset", v => ((StringNavigator)v).Reset());
         manager.RegisterMessage(this, "after", v => ((StringNavigator)v).After());
         manager.RegisterMessage(this, "rest", v => ((StringNavigator)v).Rest());
         manager.RegisterMessage(this, "any", v => ((StringNavigator)v).Any());
      }

      public Value To()
      {
         if (status == StatusType.Fail)
            return this;
         var value = Arguments[0];
         var count = (int)Arguments[1].Number;
         if (count == 0)
            count = 1;
         Action action;
         var pattern = value as Pattern;
         if (pattern != null)
            action = () => toPattern(pattern);
         else if (value.IsNumeric())
            action = () => toNumber((int)value.Number);
         else
            action = () => toString(value.Text);
         for (var i = 0; i < count; i++)
            action();
         return this;
      }

      void toPattern(Pattern pattern)
      {
         var found = pattern.IsMatch(textAtPosition());
         if (found)
         {
            var newPosition = pattern.Index;
            setRelativePosition(newPosition);
         }
         setStatus(found);
      }

      void toString(string pattern)
      {
         var newPosition = textAtPosition().IndexOf(pattern, StringComparison.Ordinal);
         var found = newPosition > -1;
         if (found)
            setRelativePosition(newPosition);
         setStatus(found);
      }

      void toNumber(int index)
      {
         setStatus(true);
         setAbsolutePosition(index);
      }

      public Value Move()
      {
         var amount = (int)Arguments[0].Number;
         setRelativePosition(amount);
         return this;
      }

      public Value Match()
      {
         if (status == StatusType.Fail)
            return this;
         var value = Arguments[0];
         var count = (int)Arguments[1].Number;
         if (count == 0)
            count = 1;
         Action action;
         var pattern = value as Pattern;
         if (pattern != null)
            action = () => matchPattern(pattern);
         else
            action = () => matchString(value.Text);
         for (var i = 0; i < count; i++)
            action();
         return this;
      }

      void matchPattern(Pattern pattern)
      {
         var found = pattern.IsMatch(textAtPosition());
         if (found)
         {
            setRelativePosition(pattern.Index);
            setAbsoluteLength(pattern.Length);
         }
         setStatus(found);
      }

      void matchString(string pattern)
      {
         var newPosition = textAtPosition().IndexOf(pattern, StringComparison.Ordinal);
         var found = newPosition > -1;
         if (found)
         {
            setRelativePosition(newPosition);
            setAbsoluteLength(pattern.Length);
         }
         setStatus(found);
      }

      public Value Open()
      {
         if (status == StatusType.Fail)
            return this;
         var value = Arguments[0];
         var count = (int)Arguments[1].Number;
         if (count == 0)
            count = 1;
         Action action;
         var pattern = value as Pattern;
         if (pattern != null)
            action = () => openPattern(pattern);
         else if (value.IsNumeric())
            action = () => openNumber((int)value.Number);
         else
            action = () => openString(value.Text);
         for (var i = 0; i < count; i++)
            action();
         return this;
      }

      void openPattern(Pattern pattern)
      {
         var found = pattern.IsMatch(textAtPosition());
         if (found)
            setAbsoluteLength(pattern.Index);
         setStatus(found);
      }

      void openString(string pattern)
      {
         var newPosition = textAtPosition().IndexOf(pattern, StringComparison.Ordinal);
         var found = newPosition > -1;
         if (found)
            setAbsoluteLength(newPosition);
         setStatus(found);
      }

      void openNumber(int length)
      {
         setAbsoluteLength(length);
      }

      public Value Slide()
      {
         var amount = (int)Arguments[0].Number;
         setRelativeLength(amount);
         return this;
      }

      public Value Right()
      {
         if (status == StatusType.Fail)
            return this;
         var count = (int)Arguments[0].Number;
         if (count == 0)
            count = 1;
         for (var i = 0; i < count; i++)
         {
            position += length;
            length = 0;
            adjust();
         }
         return this;
      }

      public Value Left()
      {
         if (status == StatusType.Fail)
            return this;
         var count = (int)Arguments[0].Number;
         if (count == 0)
            count = 1;
         for (var i = 0; i < count; i++)
         {
            position -= length;
            length = 0;
            adjust();
         }
         return this;
      }

      public Value More() => status == StatusType.More;

      public Value Reset()
      {
         position = 0;
         length = 0;
         status = StatusType.Starting;
         return this;
      }

      public Value After()
      {
         if (status == StatusType.Fail)
            return this;
         var value = Arguments[0];
         var count = (int)Arguments[1].Number;
         if (count == 0)
            count = 1;
         Action action;
         var pattern = value as Pattern;
         if (pattern != null)
            action = () => afterPattern(pattern);
         else if (value.IsNumeric())
            action = () => toNumber((int)value.Number);
         else
            action = () => afterString(value.Text);
         for (var i = 0; i < count; i++)
            action();
         return this;
      }

      void afterPattern(Pattern pattern)
      {
         var found = pattern.IsMatch(textAtPosition());
         if (found)
         {
            var newPosition = pattern.Index + pattern.Length;
            setRelativePosition(newPosition);
         }
         setStatus(found);
      }

      void afterString(string pattern)
      {
         var newPosition = textAtPosition().IndexOf(pattern, StringComparison.Ordinal);
         var found = newPosition > -1;
         if (found)
            setRelativePosition(newPosition + pattern.Length);
         setStatus(found);
      }

      void setStatus(bool found)
      {
         if (found)
            status = StatusType.Success;
         else
            switch (status)
            {
               case StatusType.Success:
                  status = StatusType.More;
                  break;
               case StatusType.Starting:
               case StatusType.More:
                  status = StatusType.Fail;
                  position = 0;
                  length = 0;
                  break;
            }
      }

      public Value Rest()
      {
         if (status == StatusType.Fail)
            return this;
         openNumber(text.Length - position);
         return this;
      }

      public Value Any()
      {
         if (status == StatusType.Fail)
            return this;
         Value value = Arguments[0].Text;
         var needle = Runtime.Expand(value.Text).ToCharArray();
         var textLength = text.Length;
         var start = position + length;
         for (var i = start; i < textLength; i++)
         {
            var s = text[i, 1][0];
            if (System.Array.IndexOf(needle, s) == -1)
            {
               if (i == position)
               {
                  setStatus(false);
                  return this;
               }
               setRelativeLength(i - start);
               setStatus(true);
               return this;
            }
            if (i == textLength - 1)
            {
               setRelativeLength(i - start);
               setStatus(true);
               return this;
            }
         }
         setStatus(false);
         return this;
      }

      public override string ToString()
      {
         var ch = ' ';
         switch (status)
         {
            case StatusType.Starting:
               ch = '!';
               break;
            case StatusType.Success:
               ch = '1';
               break;
            case StatusType.More:
               ch = '+';
               break;
            case StatusType.Fail:
               ch = '0';
               break;
         }
         return length == 0 ? $"{text[0, position]}|{text.Substring(position)}:{ch}" :
            $"{text[0, position]}[{text[position, length]}]{text.Substring(position + length)}:{ch}";
      }
   }
}