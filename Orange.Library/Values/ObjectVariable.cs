namespace Orange.Library.Values
{
   public class ObjectVariable : Variable
   {
      Object obj;
      Region region;

      public ObjectVariable(Object obj, Region region, string name)
         : base(name)
      {
         this.obj = obj;
         this.region = region;
      }

      public override Value Value
      {
         get => region[Name];
         set
         {
            if (obj.ID == value.ID)
               return;

            region[Name] = value;
         }
      }

      public override string ContainerType => ValueType.ObjectVariable.ToString();
   }
}