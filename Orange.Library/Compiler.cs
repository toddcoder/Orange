using Orange.Library.Values;
using Orange.Library.Verbs;
using Standard.Types.Collections;
using Standard.Types.Monads;

namespace Orange.Library
{
   public class Compiler
   {
      public static Compiler CompilerState { get; set; } = new Compiler();

      protected long objectID;
      protected Hash<string, UserDefinedOperator> operators;
      protected Hash<string, Trait> traits;
      protected Hash<string, Class> classes;
      protected string lastClassName;

      public Compiler()
      {
         Reset();
      }

      public void Reset()
      {
         objectID = 0;
         operators = new Hash<string, UserDefinedOperator>();
         traits = new Hash<string, Trait>();
         classes = new Hash<string, Class>();
         lastClassName = "";
      }

      public long ObjectID() => objectID++;

      public UserDefinedOperator Operator(string name) => operators[name];

      public void RegisterOperator(string name, UserDefinedOperator _operator) => operators[name] = _operator;

      public bool IsRegisteredOperator(string name) => operators.ContainsKey(name);

      public void RegisterTrait(Trait trait) => traits[trait.Name] = trait;

      public IMaybe<Trait> Trait(string name) => traits.Map(name);

      public void RegisterClass(string className, Class cls)
      {
         classes[className] = cls;
         lastClassName = className;
      }

      public IMaybe<Class> Class(string name) => classes.Map(name);

      public string LastClassName => lastClassName;
   }
}