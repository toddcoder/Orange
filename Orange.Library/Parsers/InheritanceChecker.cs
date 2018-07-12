using System.Linq;
using Orange.Library.Values;
using Orange.Library.Verbs;
using Standard.Types.Collections;
using Standard.Types.Maybe;
using static Orange.Library.Runtime;
using static Orange.Library.Compiler;
using static Orange.Library.Values.Object.VisibilityType;
using Signature = Orange.Library.Values.Signature;

namespace Orange.Library.Parsers
{
   public class InheritanceChecker
   {
      string className;
      bool isAbstract;
      string[] traitNames;
      IMaybe<Class> superClass;
      Hash<string, Signature> signatures;
      Hash<string, Signature> superSignatures;
      Hash<string, Signature> abstractSignatures;
      Hash<string, bool> overrides;
      Hash<string, Trait> traits;

      public InheritanceChecker(string className, Block objectBlock, Parameters parameters, string superClassName,
         bool isAbstract, string[] traitNames)
      {
         this.className = className;
         this.isAbstract = isAbstract;
         this.traitNames = traitNames;

         superClass = CompilerState.Class(superClassName);
         if (superClass.IsSome || traitNames.Length > 0)
         {
            signatures = getSignatures(objectBlock, parameters);
            if (superClass.IsSome)
            {
               var superObjectBlock = superClass.Value.ObjectBlock;
               superSignatures = getSignatures(superObjectBlock, superClass.Value.Parameters);
               abstractSignatures = superObjectBlock.AsAdded
                  .OfType<SpecialAssignment>()
                  .Where(a => a.IsAbstract)
                  .Select(a => ((Abstract)a.Value).Signature)
                  .ToHash(s => s.Name);
            }
            else
            {
               superSignatures = new Hash<string, Signature>();
               abstractSignatures = new Hash<string, Signature>();
            }
            overrides = objectBlock.AsAdded.OfType<CreateFunction>().ToHash(cf => cf.FunctionName, cf => cf.Override);
            foreach (var name in parameters.GetParameters().Select(parameter => parameter.Name))
            {
               overrides[name] = true;
               overrides[SetterName(name)] = true;
            }
         }
      }

      static Hash<string, Signature> getSignatures(Block block, Parameters parameters)
      {
         var signatures = new Hash<string, Signature>();

         foreach (var verb in block.AsAdded)
            switch (verb)
            {
               case CreateFunction createFunction:
                  signatures[createFunction.FunctionName] = createFunction.Signature;
                  break;
               case SpecialAssignment sa when sa.Signature.If(out var signature):
                  signatures[signature.Name] = signature;
                  break;
               case AssignToNewField assign:
                  var fieldName = assign.FieldName;
                  signatures[fieldName] = new Signature(fieldName, 0, false);
                  if (assign.ReadOnly)
                     signatures[SetterName(fieldName)] = new Signature(fieldName, 1, false);
                  break;
            }

         foreach (var parameter in parameters.GetParameters())
         {
            var name = parameter.Name;
            var visibility = parameter.Visibility;
            if (visibility == Temporary)
               continue;

            signatures[name] = new Signature(name, 0, false);
            if (!parameter.ReadOnly)
               signatures[SetterName(name)] = new Signature(name, 1, false);
         }

         return signatures;
      }

      public IResult<string> Passes()
      {
         if (superClass.IsNone && traitNames.Length == 0)
            return className.Success();

         return
            from checkedTraits in checkTraits()
            from checkedAbstracts in checkAbstracts()
            from checkedOverrides in checkOverrides()
            select checkedOverrides;
      }

      IResult<string> checkTraits()
      {
         traits = new Hash<string, Trait>();
         foreach (var traitName in traitNames)
         {
            var trait = CompilerState.Trait(traitName);
            if (trait.IsNone)
               return $"{traitName} is not a trait".Failure<string>();

            traits[traitName] = trait.Value;
         }

         // ReSharper disable once LoopCanBePartlyConvertedToQuery
         foreach (var item in traits)
         {
            var traitName = item.Key;
            var trait = item.Value;
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var member in trait.Members)
               if (member.Value is Signature signature && !signatures.ContainsKey(signature.Name))
                  return $"Trait {traitName}.{signature.UnmangledSignature} not defined in class {className}".Failure<string>();
         }

         return className.Success();
      }

      IResult<string> checkAbstracts()
      {
         if (isAbstract)
            return className.Success();

         foreach (var absSignature in abstractSignatures
            .Select(item => new { item, absSignature = item.Value })
            .Select(t => new { t, signature = signatures.Map(t.item.Key) })
            .Where(t => !t.signature.IsSome || t.signature
               .FlatMap(s => s.IfCast<SpecialAssignment>()
                  .FlatMap(sa => sa.IsAbstract, () => false), () => false))
            .Select(t => t.t.absSignature))
            return $"{absSignature.UnmangledSignature} hasn't been implemented".Failure<string>();

         return className.Success();
      }

      IResult<string> checkOverrides()
      {
         foreach (var item in signatures.Where(item => !isOverridden(item.Key)))
            return $"{item.Key} needs to be overridden".Failure<string>();

         return className.Success();
      }

      bool isOverridden(string name) => !superSignatures.ContainsKey(name) || overrides.DefaultTo(name, false);
   }
}