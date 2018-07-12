using Orange.Library.Values;

namespace Orange.Library
{
   public interface INSGenerator
   {
      void Reset();

      Value DoReset();

      Value Next();

      Value If();

      Value IfNot();

      Value Map();

      Value MapIf();

      Value Skip();

      Value SkipWhile();

      Value SkipUntil();

      Value Take();

      Value TakeWhile();

      Value TakeUntil();

      Value Group();

      Value Array();

      Value FoldL();

      Value FoldR();

      Value AnyOf();

      Value AllOf();

      Value OneOf();

      Value NoneOf();

      Value Split();

      Value SplitWhile();

      Value SplitUntil();

      Value Unique();

      Value Flat();

      Value First();

      Region Region { get; set; }

      INSGeneratorSource GeneratorSource { get; }

      void Visit(Value value);

      bool More { get; }
   }
}