using System.Collections.Generic;
using Orange.Library.Values;
using Orange.Library.Verbs;
using static Orange.Library.Values.Object.VisibilityType;
using static Orange.Library.Compiler;

namespace Orange.Library.Parsers
{
   public class CofunctionBuilder
   {
      const string VAR_STATE = "__$state";
      const string VAR_SOURCE = "__$source";

      static Block[] getBlocks(Block block, CodeBuilder fieldBuilder, CodeBuilder resetBuilder)
      {
         var builder = new CodeBuilder();
         var blocks = new List<Block>();

         foreach (var verb in block.AsAdded)
         {
            switch (verb)
            {
               case Yield _yield:
                  builder.Defer(_yield.Expression);
                  builder.AssignToField(VAR_STATE, blocks.Count + 1, 0);
                  blocks.Add(builder.Block);
                  builder.Clear();
                  continue;
               case ForExecute _for:
                  fieldBuilder.CreateField(_for.Parameters[0].Name, false, Private);

                  resetBuilder.Push();
                  resetBuilder.CreateGenerator();
                  resetBuilder.Parenthesize(_for.Value);
                  var expression = resetBuilder.Pop(true);

                  resetBuilder.AssignToField(VAR_SOURCE, expression);
                  continue;
               case AssignToNewField assignToNewField when !assignToNewField.ReadOnly:
                  var fieldName = assignToNewField.FieldName;
                  fieldBuilder.CreateField(fieldName, false, Private);
                  builder.AssignToField(fieldName, assignToNewField.Expression);
                  continue;
            }

            builder.Verb(verb);
         }

         builder.ReturnNil();
         blocks.Add(builder.Block);

         return blocks.ToArray();
      }

      static Block getMatch(Block[] blocks)
      {
         var builder = new CodeBuilder();
         builder.Push();
         var index = 0;

         foreach (var block in blocks)
            builder.Case(new Double(index++).Pushed, block);

         builder.Push();
         builder.ReturnNil();
         var returnNil = builder.Pop(false);
         builder.Case(new Double(index).Pushed, returnNil);

         var actions = builder.Pop(false);
         builder.Push();
         builder.Variable(VAR_STATE);
         var target = builder.Pop(true);
         builder.Match(target, actions, "");

         return builder.Block;
      }

      string functionName;
      Parameters parameters;
      Block body;

      public CofunctionBuilder(string functionName, Parameters parameters, Block body)
      {
         this.functionName = functionName;
         this.parameters = parameters;
         this.body = body;
      }

      public Verb Generate()
      {
         var fieldBuilder = new CodeBuilder();
         fieldBuilder.CreateField(VAR_STATE, false, Private);
         fieldBuilder.CreateField(VAR_SOURCE, false, Private);

         var resetBuilder = new CodeBuilder();
         resetBuilder.AssignToField(VAR_STATE, 0, 0);

         var blocks = getBlocks(body, fieldBuilder, resetBuilder);
         var matchBlock = getMatch(blocks);

         fieldBuilder.Function("reset", resetBuilder);
         fieldBuilder.Function("next", new Lambda(new Region(), matchBlock, new Parameters(), false));

         var objectBlock = fieldBuilder.Block;

         var cls = new Class(parameters, objectBlock);
         CompilerState.RegisterClass(functionName, cls);
         return new CreateClass(functionName, cls);
      }
   }
}