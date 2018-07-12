using static Orange.Library.Managers.RegionManager;

namespace Orange.Library
{
   public class SharedRegionPopper : RegionPopper
   {
      protected Region sharedRegion;

      public SharedRegionPopper(Region region, Region sharedRegion, string name)
         : base(region, name) => this.sharedRegion = sharedRegion;

      public SharedRegionPopper(Region region, ISharedRegion sharedRegionHost, string name)
         : base(region, name) => sharedRegion = sharedRegionHost.SharedRegion;

      public override void Push()
      {
         if (sharedRegion != null)
            Regions.Push(sharedRegion, $"shared-{name}");
         base.Push();
      }

      public override void Pop()
      {
         base.Pop();
         if (sharedRegion != null)
            Regions.Pop($"shared-{name}");
      }
   }
}