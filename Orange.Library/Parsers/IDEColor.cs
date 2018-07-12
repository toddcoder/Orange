namespace Orange.Library.Parsers
{
   public class IDEColor
   {
      public enum EntityType
      {
         Strings,
         Numbers,
         Operators,
         Variables,
         Structures,
         Whitespaces,
         Comments,
         Messaging,
         Formats,
         Dates,
         Arrays,
         Alternators,
         Symbols,
         Booleans,
         KeyWords,
         Invokeables,
         Interpolated,
         Types
      }

      public int Position { get; set; }

      public int Length { get; set; }

      public EntityType Type { get; set; }

      public string Key => $"{Position}:{Length}";

      public bool Bold { get; set; }

      public override string ToString() => $"{Key}@{Type}{(Bold ? "b" : "")}";

      public string Slice(string code) => code.Substring(Position, Length);
   }
}