using System.Collections.Generic;

namespace Orange.Library.Managers
{
   public class ElseManager
   {
      Stack<bool> elses;

      public ElseManager() => elses = new Stack<bool>();

      public void IfSuccess()
      {
         elses.Push(false);
      }

      public void IfFail()
      {
         elses.Push(true);
      }

      public bool PendingElse()
      {
         var pending = elses.Pop();
         if (pending)
            return true;
         elses.Push(false);
         return false;
      }
   }
}