using System.Collections.Generic;
using System.Linq;
using Orange.Library.Verbs;
using Standard.Types.Enumerables;
using static Orange.Library.Runtime;
using static Orange.Library.Values.Parameters;

namespace Orange.Library.Values
{
   public class ExpansibleLambda : MultiLambda
   {
      class ExpansibleItem
      {
         static void setPlaceholderNames(Parameters parameters)
         {
            foreach (var parameter in parameters.GetParameters().Where(p => (p.Comparisand?.Count ?? 0) != 0))
            {
               var comparisandAsAdded = parameter.Comparisand.AsAdded[0];
               if (comparisandAsAdded is FunctionInvoke functionInvoke)
               {
                  parameter.PlaceholderName = functionInvoke.Arguments.Blocks
                     .Where(b => b.Count > 0)
                     .Select(b => b[0])
                     .Where(v => v is Push)
                     .Cast<Push>()
                     .Where(p => p.Value is Placeholder)
                     .Select(p => p.Value)
                     .Select(v => v.Text)
                     .Listify(",");
                  continue;
               }

               if (comparisandAsAdded is Push push)
                  switch (push.Value)
                  {
                     case Placeholder placeholder:
                        parameter.PlaceholderName = placeholder.Text;
                        break;
                     case Array array:
                        parameter.PlaceholderName = array.Values.Where(v => v is Placeholder).Select(v => v.Text).Listify(",");
                        break;
                     case List list:
                        parameter.PlaceholderName = list.ComparisonValues().Where(v => v is Placeholder).Select(v => v.Text).Listify(":");
                        break;
                  }
            }
         }

         Parameters parameters;
         VerbList verbs;
         Block condition;

         public ExpansibleItem(MultiLambdaItem item)
         {
            var lambda = item.Lambda;
            parameters = lambda.Parameters;
            setPlaceholderNames(parameters);
            verbs = new VerbList(lambda.Block);
            condition = item.Condition;
         }

         public Parameters Parameters => parameters;

         public VerbList Verbs => verbs;

         public List<Verb> CurrentVerbs { get; set; }

         public Block Condition => condition;
      }

      const string LOCATION = "Expansible lambda";

      List<ExpansibleItem> expansibleItems;

      public ExpansibleLambda(string functionName, bool memoize)
         : base(functionName, memoize) => expansibleItems = new List<ExpansibleItem>();

      public override void Add(MultiLambdaItem item) => expansibleItems.Add(new ExpansibleItem(item));

      public override Value Invoke(Arguments arguments)
      {
         var current = new VerbList();

         arguments.DefaultValue = new Nil();
         using (var popper = new RegionPopper(new Region(), "expand-lambda"))
         {
            popper.Push();
            for (var i = 0; i < MAX_RECURSION; i++)
               foreach (var item in expansibleItems)
               {
                  var parameters = item.Parameters;
                  var values = parameters.GetArguments(arguments);
                  popper.Push();
                  SetArguments(values);
                  if (canInvoke(parameters, values, false))
                  {
                     ExecuteWhere(this);
                     if (parameters.Condition != null && !parameters.Condition.Evaluate().IsTrue)
                        continue;
                     if (item.Condition != null && !item.Condition.Evaluate().IsTrue)
                        continue;

                     if (current.IsEmpty)
                        current.Add(item.Verbs);
                     else
                        current.ReplacePlaceholderInvokeWithBody(item.Verbs);
                     current.ReplaceParameters(parameters);
                     current.ReplaceArrayParameters(parameters);
                     current.ReplaceListParameters(parameters);
                     if (!current.ReplaceInvocationWithPlaceholder(functionName, ref arguments))
                        return current.Block;
                  }
               }

            return new Nil();
         }
      }

      public override string ToString() => expansibleItems.Listify();
   }
}