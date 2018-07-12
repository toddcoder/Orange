using Microsoft.VisualStudio.TestTools.UnitTesting;
using static System.Console;

namespace Orange.Test
{
   [TestClass]
   public class OrangeTest
   {
      [TestMethod]
      public void ParsingTest()
      {
         var orange = new Library.Orange("'foo'.len");
         var result = orange.Execute();
         var value = orange.LastValue;
         var type = orange.LastType;
         WriteLine(result);
         WriteLine($"{value} | {type}");
      }
   }
}
