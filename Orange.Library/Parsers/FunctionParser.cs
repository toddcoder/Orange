using System.Collections.Generic;
using System.Text;
using Orange.Library.Parsers.Special;
using Orange.Library.Values;
using Orange.Library.Verbs;
using Standard.Types.Strings;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Parsers.Special.ParametersParser;
using static Orange.Library.Runtime;
using static Orange.Library.Values.Object;
using static Orange.Library.Values.Object.VisibilityType;
using static Standard.Types.Maybe.MaybeExtensions;

namespace Orange.Library.Parsers
{
   public class FunctionParser : Parser, ITraitName
   {
      FunctionBodyParser functionBodyParser;
      string functionName;
      bool singleLine;
      VisibilityType visibility;
      bool _override;
      bool lockedDown;
      Lambda lambda;
      Parameters parameters;
      WhereParser whereParser;
      int statementIndex;

      public FunctionParser()
         : base("^ /(|tabs|) /(('public' | 'hidden' | 'imported') /s+)? /('override' /s+)? /('memo' /s+)?" +
            "/(('x'? 'func' | 'get' | 'set' | 'init' | 'pure' | 'before' | 'after' | 'rec' | 'require' | 'ensure' " +
            $"| 'invariant' | 'cofunc') |sp+|) /({REGEX_VARIABLE}) /(['(:['])")
      {
         functionBodyParser = new FunctionBodyParser();
         whereParser = new WhereParser();
      }

      public override Verb CreateVerb(string[] tokens)
      {
         var whitespaces = tokens[1];
         statementIndex = position + whitespaces.Length;

         var tokens2Length = tokens[2].Length;
         var visibilityName = tokens[2].Trim();
         visibility = ParseVisibility(visibilityName);

         var tokens3Length = tokens[3].Length;
         _override = tokens3Length > 0;

         var tokens4Length = tokens[4].Length;
         var memoize = tokens4Length > 0;

         var tokens5Length = tokens[5].Length;
         var type = tokens[5].Trim();

         var tokens6Length = tokens[6].Length;
         functionName = LongToMangledPrefix(type, tokens[6].Trim());

         var parameterType = tokens[7];
         var parametersType = ParametersType.Message;
         switch (parameterType)
         {
            case "(":
               parametersType = ParametersType.Standard;
               break;
            case "[":
               parametersType = ParametersType.Pattern;
               break;
         }

         var xMethod = false;
         var isDef = true;
         lockedDown = false;
         var isInit = false;
         var isCoFunc = false;

         switch (type)
         {
            case "cofunc":
               isCoFunc = true;
               break;
            case "xfunc":
               xMethod = true;
               break;
            case "pure":
               lockedDown = true;
               break;
            case "init":
               isDef = false;
               isInit = true;
               break;
         }

         Reject(LockedDown && type == "set", VerboseName, "Setters not allowed in views");
         Reject(!InClassDefinition && visibilityName.IsNotEmpty(), VerboseName,
            $"Visibility specifier {visibility} not allowed allowed outside of class definition");
         Reject(!InClassDefinition && !isDef, VerboseName, $"{type} specifier not allowed outside of a class definition");
         Reject(InClassDefinition && xMethod, VerboseName, "xfunc not allowed inside class definitions");
         Color(position, whitespaces.Length, Whitespaces);
         Color(tokens2Length, KeyWords);
         Color(tokens3Length, KeyWords);
         Color(tokens4Length, KeyWords);
         Color(tokens5Length, KeyWords);
         var invokable = parametersType == ParametersType.Standard || parametersType == ParametersType.Pattern;
         var entityType = invokable ? Invokeables : Messaging;
         Color(tokens6Length, entityType);
         Color(parameterType.Length, Structures);

         var index = NextPosition;
         var parametersParser = new ParametersParser(parametersType);
         if (parametersParser.Parse(source, index).If(out var parsed))
         {
            int newIndex;
            (parameters, newIndex) = parsed;
            index = newIndex;
            if (source.Skip(index).Take(1) == "(")
            {
               var curriedFunctionParser = new CurriedFunctionParser(functionName, parameters, visibility, _override);
               if (curriedFunctionParser.Scan(source, index))
               {
                  overridePosition = curriedFunctionParser.Position;
                  return curriedFunctionParser.Verb;
               }
            }
         }
         else
         {
            var builder = new StringBuilder();
            var messageParameterParser = new MessageParameterParser();
            var variableParser = new VariableParser();
            var parameterList = new List<Parameter>();
            if (variableParser.Scan(source, index))
            {
               var variable = (Variable)variableParser.Value;
               var parameter = new Parameter(variable.Name);
               builder.Append(functionName);
               builder.Append("_");
               parameterList.Add(parameter);
               index = variableParser.Position;
            }
            else
               return null;

            while (messageParameterParser.Scan(source, index))
            {
               var parameter = new Parameter(messageParameterParser.ParameterName);
               parameterList.Add(parameter);
               builder.Append(messageParameterParser.MessageName);
               builder.Append("_");
               index = messageParameterParser.Result.Position;
            }

            functionName = builder.ToString();
            parameters = new Parameters(parameterList);
         }

         RejectNull(parameters, VerboseName, "Parameters malformed");
         var currying = parametersParser.Currying;

         functionBodyParser.ExtractCondition = parametersType == ParametersType.Pattern;
         if (functionBodyParser.Parse(source, index).If(out var block, out var i))
         {
            index = i;
            var condition = functionBodyParser.Condition;
            var where = functionBodyParser.Where;
            Verb verb;
            if (isInit)
            {
               verb = createInitializer(index, block);
               result.Value = lambda;
               singleLine = !functionBodyParser.MultiCapable;
               return verb;
            }

            if (isCoFunc)
            {
               overridePosition = index;
               var builder = new CofunctionBuilder(functionName, parameters, block);
               return builder.Generate();
            }

            verb = createFunction(index, currying, condition, memoize, lockedDown, block);
            lambda.XMethod = xMethod;
            lambda.Expand = type == "rec";
            result.Value = lambda;
            singleLine = !functionBodyParser.MultiCapable;
            if (lambda.Where == null)
               lambda.Where = where;
            return verb;
         }

         return null;
/*         return functionBodyParser.Parse(source, index).Map((block, i) =>
         {
            index = i;
            var condition = functionBodyParser.Condition;
            var where = functionBodyParser.Where;
            Verb verb;
            if (isInit)
            {
               verb = createInitializer(index, block);
               result.Value = lambda;
               singleLine = !functionBodyParser.MultiCapable;
               return verb;
            }

            if (isCoFunc)
            {
               overridePosition = index;
               var builder = new CofunctionBuilder(functionName, parameters, block);
               return builder.Generate();
            }

            verb = createFunction(index, currying, condition, memoize, lockedDown, block);
            lambda.XMethod = xMethod;
            lambda.Expand = type == "rec";
            result.Value = lambda;
            singleLine = !functionBodyParser.MultiCapable;
            if (lambda.Where == null)
               lambda.Where = where;
            return verb;
         }, () => null);*/
      }

      Verb createFunction(int index, bool currying, Block condition, bool memoize, bool lockedDown, Block body) =>
         createFunction(index, body, currying, condition, memoize, lockedDown);

      Verb createFunction(int index, Block block, bool currying, Block inlineCondition, bool memoize, bool isLockedDown)
      {
         var region = getRegion();
         if (currying)
            lambda = new CurryingLambda(region, block, parameters, false);
         else if (memoize && isLockedDown)
            lambda = new MemoLambda(region, block, parameters, false);
         else
            lambda = new Lambda(region, block, parameters, true);
         var condition = inlineCondition;
         if (condition is null)
            if (ConditionParser.Parse(source, index).If(out var b, out var i))
            {
               index = i;
               condition = b;
            }
            else
               condition = null;

         if (whereParser.Scan(source, index))
         {
            var where = (Block)whereParser.Value;
            lambda.Where = where;
            index = whereParser.Position;
         }
         overridePosition = index;
         return new CreateFunction(functionName, lambda, lambda.Parameters.Multi, visibility, _override, condition, memoize: memoize)
            { Index = statementIndex };
      }

      Region getRegion() => lockedDown ? new LockedDownRegion(functionName) : new Region();

      Verb createInitializer(int index, Block block)
      {
         overridePosition = index;
         var region = getRegion();
         lambda = new Lambda(region, block, parameters, false) { Initializer = true };
         var builder = new CodeBuilder();
         var verb = new CreateFunction(functionName, lambda, false, Public, true, null) { Index = statementIndex };
         builder.Verb(verb);
         AddStaticBlock(builder.Block);
         return new NullOp();
      }

      public override string VerboseName => "function";

      public string MemberName => functionName;

      public Lambda Getter => null;

      public Lambda Setter => null;

      public override bool EndOfBlock => InStatic && singleLine;
   }
}