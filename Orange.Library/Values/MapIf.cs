using Orange.Library.Managers;

namespace Orange.Library.Values
{
   public class MapIf : Value
   {
      Arguments map;
      Arguments _if;

      public MapIf(Arguments map, Arguments _if)
      {
         this.map = map;
         this._if = _if;
      }

      public Arguments Map => map;

      public Arguments If => _if;

      public override int Compare(Value value) => 0;

      public override string Text { get; set; }

      public override double Number { get; set; }

      public override ValueType Type => ValueType.MapIf;

      public override bool IsTrue => false;

      public override Value Clone() => new MapIf(map.Clone(), _if.Clone());

      protected override void registerMessages(MessageManager manager) { }

      public override string ToString() => $"{map} -? {_if}";
   }
}