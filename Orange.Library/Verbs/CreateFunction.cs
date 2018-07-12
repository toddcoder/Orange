using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Managers.RegionManager;
using static Orange.Library.Runtime;
using static Orange.Library.Values.Object;
using static Orange.Library.Values.Object.VisibilityType;
using Signature = Orange.Library.Values.Signature;

namespace Orange.Library.Verbs
{
   public class CreateFunction : Verb, IStatement
   {
      string functionName;
      Lambda lambda;
      bool multiCapable;
      VisibilityType visibility;
      bool _override;
      Block condition;
      bool autoInvoke;
      bool memoize;
      bool global;
      string result;

      public CreateFunction(string functionName, Lambda lambda, bool multiCapable, VisibilityType visibility,
         bool _override, Block condition, bool autoInvoke = false, bool memoize = false, bool global = false)
      {
         this.functionName = functionName;
         this.lambda = lambda;
         this.multiCapable = multiCapable;
         this.visibility = visibility;
         this._override = _override;
         this.condition = condition;
         this.autoInvoke = autoInvoke;
         this.memoize = memoize;
         this.global = global;
         result = "";
      }

      public CreateFunction()
         : this("", null, false, Public, false, null) { }

      public override Value Evaluate()
      {
         if (multiCapable)
         {
            MultiLambda multiLambda = null;
            InvokeableReference reference = null;
            var isAReference = false;
            if (Regions.VariableExists(functionName))
            {
               var value = Regions[functionName];

               //isAReference = value.As<InvokeableReference>().Assign(out reference);
               if (value is InvokeableReference invokeableReference)
               {
                  isAReference = true;
                  var invokeable = invokeableReference.Invokeable;
                  RejectNull(invokeable, "Create function", "Not an invokeable");
                  value = (Value)invokeable;
               }
               if (!(value is MultiLambda multi))
               {
                  var lambdaItem = (Lambda)value;
                  var multiLambdaItem = new MultiLambdaItem(lambdaItem, false, condition);
                  multi = lambdaItem.Expand ? new ExpansibleLambda(functionName, memoize) : new MultiLambda(functionName, memoize);
                  multi.Add(multiLambdaItem);
                  multiLambda = multi;
               }
            }
            else
            {
               multiLambda = lambda.Expand ? new ExpansibleLambda(functionName, memoize) : new MultiLambda(functionName, memoize);
               Regions.CreateVariable(functionName, global, visibility, _override);
            }
            if (multiLambda is null)
               return null;

            var item = new MultiLambdaItem(lambda, false, condition);
            multiLambda.Add(item);
            if (isAReference)
               reference.Invokeable = multiLambda;
            else
               Regions[functionName] = multiLambda;
            if (lambda.Where != null)
               multiLambda.Where = lambda.Where;
            MarkAsXMethod(functionName, lambda);
            lambda.Message = functionName;
            result = representation();
            return null;
         }

         if (_override)
            Regions.UnsetReadOnly(functionName);
         Regions.CreateVariable(functionName, global, visibility, _override);
         var assignedValue = autoInvoke ? (Value)new AutoInvoker(lambda) : lambda;
         Regions[functionName] = assignedValue;
         Regions.SetReadOnly(functionName);
         MarkAsXMethod(functionName, lambda);
         result = representation();
         return null;
      }

      string representation() => $"func {functionName}({lambda.Parameters})";

      public override VerbPresidenceType Presidence => VerbPresidenceType.Statement;

      public string FunctionName => functionName;

      public Lambda Lambda => lambda;

      public bool IsEnd => !multiCapable;

      public bool EvaluateFirst => true;

      public override string ToString() => $"func {functionName}";

      public Signature Signature => new Signature(functionName, lambda.ParameterCount, false);

      public string Result => result;

      public string TypeName => "Lambda";

      public int Index { get; set; }

      public bool Override => _override;
   }
}