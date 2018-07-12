using Orange.Library.Values;
using static Orange.Library.Values.Object;

namespace Orange.Library
{
   public class LambdaRegion : Region
   {
      public LambdaRegion(Region region)
      {
         variables = region.Variables;
         deinitializations = region.Deinitializations;
         readonlys = region.ReadOnlys;
         visibilityTypes = region.VisibilityTypes;
         initializers = region.Initializers;
      }

      public override bool Exists(string name) => false;

      public override void CreateVariable(string variableName, bool global = false,
         VisibilityType visibility = VisibilityType.Public, bool _override = false)
      {
         variables[variableName] = new Nil();
         visibilityTypes[variableName] = visibility;
      }
   }
}