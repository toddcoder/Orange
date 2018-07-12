using System.Collections.Generic;
using System.IO;

namespace Orange.Library.Templates
{
   public class XMLEnd : Item, IXMLStack
   {
      public XMLEnd()
         : base("")
      {
      }

      public Stack<string> Stack
      {
         get;
         set;
      }

      public override void Render(StringWriter writer, string variableName)
      {
         var name = Stack.Pop();
         writer.WriteLine("|</{0}>|.write('{1}');", name, variableName);
         writer.WriteLine("''.print('{0}');", variableName);
      }
   }
}