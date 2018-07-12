using static Orange.Library.Managers.RegionManager;

namespace Orange.Library
{
   public class DoubleRegionPopper : RegionPopper
   {
      protected Region temporary;

      public DoubleRegionPopper(Region region, string name)
         : base(region, name) => temporary = new Region();

      public override void Push()
      {
         base.Push();
         Regions.Push(temporary, $"temp-{name}");
      }

      public override void Pop()
      {
         Regions.Pop($"temp-{name}");
         base.Pop();
      }
   }
}