using Orange.Library.Managers;
using static Orange.Library.Runtime;
using static Orange.Library.Values.Nil;

namespace Orange.Library.Values
{
   public class Scanner : Value, INSGeneratorSource, ISharedRegion
   {
      public class ScannerGenerator : NSGenerator
      {
         bool oneReset;

         public ScannerGenerator(INSGeneratorSource generatorSource)
            : base(generatorSource) => oneReset = true;

         public override void Reset()
         {
            if (oneReset)
            {
               base.Reset();
               oneReset = false;
            }
         }

         public override void Visit(Value value)
         {
            if (value.Type == ValueType.Ignore || value.IsNil)
               index--;
         }
      }

      INSGeneratorSource source;
      string variableName;
      Block block;
      Region region;
      INSGenerator generator;
      NSGenerator sourceGenerator;

      public Scanner(INSGeneratorSource source, Lambda lambda)
      {
         this.source = source;
         decomposeLambda(lambda);
         region = new Region();
      }

      public Scanner(INSGeneratorSource source, string variableName, Block block)
      {
         this.source = source;
         this.variableName = variableName;
         this.block = block;
         region = new Region();
      }

      void decomposeLambda(Lambda lambda)
      {
         var parameters = lambda.Parameters.GetParameters();
         variableName = parameters.Length == 0 ? MangledName("0") : parameters[0].Name;
         block = lambda.Block;
      }

      public override int Compare(Value value) => 0;

      public override string Text
      {
         get;
         set;
      } = "";

      public override double Number
      {
         get;
         set;
      }

      public override ValueType Type => ValueType.Scanner;

      public override bool IsTrue => true;

      public override Value Clone() => new Scanner(source, variableName, (Block)block.Clone());

      protected override void registerMessages(MessageManager manager)
      {
      }

      public INSGenerator GetGenerator() => new NSGenerator(this);

      public Value Next(int index)
      {
         if (index == 0)
         {
            generator = block.GetGenerator();
            sourceGenerator = new ScannerGenerator(source);
            generator.Reset();
            sourceGenerator.Reset();
         }
         using (var popper = new SharedRegionPopper(region, this, "scanner-next"))
         {
            popper.Push();

            region.SetParameter(variableName, sourceGenerator);

            var value = generator.Next();
            return value.IsNull ? NilValue : value;
         }
      }

      public bool IsGeneratorAvailable => true;

      public Array ToArray() => GeneratorToArray(this);

      public override string ToString() => ToArray().ToString();

      public Region SharedRegion
      {
         get;
         set;
      }
   }
}