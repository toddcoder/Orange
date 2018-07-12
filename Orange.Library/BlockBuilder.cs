using System.Collections.Generic;
using Orange.Library.Verbs;
using static Orange.Library.Managers.ExpressionManager;

namespace Orange.Library
{
   public class BlockBuilder
   {
      VerbStack stack;
      List<Verb> verbs;
      List<Verb> asAdded;

      public BlockBuilder() => Clear();

      public void Add(Verb verb)
      {
         if (verb is End)
         {
            if (asAdded.Count == 0)
               return;

            asAdded.Add(verb);

            endOfBlock();
            verbs.Add(verb);
            return;
         }

         asAdded.Add(verb);

         while (stack.PendingReady(verb))
         {
            var pendingVerb = stack.Pop();
            verbs.Add(pendingVerb);
         }

         if (verb.Presidence == VerbPresidenceType.Push)
            verbs.Add(verb);
         else
            stack.Push(verb);
      }

      void endOfBlock()
      {
         while (!stack.IsEmpty)
         {
            var verb = stack.Pop();
            verbs.Add(verb);
         }
      }

      public List<Verb> Verbs
      {
         get
         {
            endOfBlock();
            return verbs;
         }
      }

      public List<Verb> AsAdded => asAdded;

      public void Clear()
      {
         stack = new VerbStack();
         verbs = new List<Verb>();
         asAdded = new List<Verb>();
      }

      public void Refresh()
      {
         stack = new VerbStack();
         verbs = new List<Verb>();
         var newList = asAdded;
         asAdded = new List<Verb>();
         foreach (var verb in newList)
            Add(verb);
      }
   }
}