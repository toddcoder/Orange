using System.Collections.Generic;
using System.Linq;
using Orange.Library.Values;
using Standard.Types.Collections;
using Standard.Types.RegularExpressions;
using static Orange.Library.Managers.RegionManager;
using static Orange.Library.Runtime;
using static Orange.Library.Values.Object;

namespace Orange.Library
{
   public class ObjectBuildingRegion : Region
   {
      const string LOCATION = "Object building namespace";

      string className;
      List<Block> staticBlocks;
      Hash<string, IInvokeable> invariants;

      public ObjectBuildingRegion(string className)
      {
         this.className = className;
         staticBlocks = new List<Block>();
         invariants = new Hash<string, IInvokeable>();
      }

      public string InvokeableName(string name) => Object.InvokeableName(className, IsObject, name);

      public override Value this[string name]
      {
         get => base[name];
         set
         {
            if (IsSpecialVariable(name))
            {
               if (Empty)
                  return;

               base[name] = value;
               return;
            }

            Assert(variables.ContainsKey(name), LOCATION, $"Variable {name} undefined");
            switch (value)
            {
               case IInvokeable invokeable:
                  if (variables[name] is Abstract anAbstract)
                     Assert(invokeable.Matches(anAbstract.Signature), LOCATION, $"Signature for {name} doesn't match");
                  getContractInvokeable(name, invokeable);
                  addPossibleInvariant(name, invokeable);
                  var invokeableName = InvokeableName(name);
                  invokeable.ImmediatelyInvokeable = true;
                  State.SetInvokeable(invokeableName, invokeable);
                  SetVariable(name, new InvokeableReference(invokeableName));
                  break;
               case InvokeableReference reference:
                  SetVariable(name, reference);
                  break;
               default:
                  SetVariable(name, value);
                  break;
            }
         }
      }

      void setAuto(string name)
      {
         if (name.Matches($"^ '__$get_' /({REGEX_VARIABLE})").If(out var m))
            SetVariable(m.FirstGroup, new InternalGetter(name));
      }

      void getContractInvokeable(string name, IInvokeable invokeable)
      {
         if (!IsPrefixed(name, out var type, out var plainName))
            return;

         if (!type.IsMatch("^ 'req' | 'ens' $"))
            return;

         Assert(variables.ContainsKey(plainName), LOCATION,
            $"Invokeable {plainName} must be defined before any of its contract terms");
         var mainValue = variables[plainName];

         if (mainValue is InvokeableReference mainReference)
         {
            var mainInvokeable = mainReference.Invokeable;
            if (mainInvokeable is ContractInvokeable contractInvokeable)
            {
               var newInvokeable = new ContractInvokeable { Main = mainInvokeable, ImmediatelyInvokeable = true, Name = plainName };
               State.SetInvokeable(mainReference.VariableName, newInvokeable);
               SetVariable(plainName, mainReference);
               switch (type)
               {
                  case "req":
                     contractInvokeable.Require = invokeable;
                     break;
                  case "ens":
                     contractInvokeable.Ensure = invokeable;
                     break;
               }
            }
         }
         else
            Throw(LOCATION, $"{plainName} must be an invokeable");
      }

      void addPossibleInvariant(string name, IInvokeable invokeable)
      {
         if (!IsPrefixed(name, out var type, out var plainName))
            return;

         if (type != "inv")
            return;

         var setterName = VAR_MANGLE + "set_" + plainName;
         Assert(variables.ContainsKey(setterName) || variables.ContainsKey(plainName), LOCATION,
            $"Field {plainName} for invariant must be defined first");
         invariants[plainName] = invokeable;
      }

      public ObjectRegion NewRegion(Object obj, bool purgeTempVariables)
      {
         lockVariables();
         var region = new ObjectRegion(obj, invariants);
         foreach (var item in variables)
            region.SetLocal(item.Key, item.Value, visibilityTypes[item.Key]);
         foreach (var item in ReadOnlys.Where(item => item.Value == ReadOnlyType.ReadOnly))
            region.SetReadOnly(item.Key);

         if (purgeTempVariables)
            purgeTemporaryVariables(region);
         region.SetInitializers(Initializers);
         return region;
      }

      void purgeTemporaryVariables(Region region)
      {
         var list = region.Variables.Where(i => isTemporary(i.Key)).Select(i => i.Key).ToArray();
         foreach (var key in list)
            region.Remove(key);
      }

      void lockVariables()
      {
         var list = variables.Where(i => isLocked(i.Key)).Select(i => i.Key).ToArray();
         foreach (var key in list)
         {
            visibilityTypes[key] = VisibilityType.Public;
            SetReadOnly(key);
         }
      }

      public void DetectAbstracts()
      {
         foreach (var item in Variables)
            Reject(item.Value.Type == Value.ValueType.Abstract, LOCATION, $"Abstract {item.Key} hasn't been redefined");
      }

      public void DetectToDos()
      {
         foreach (var item in Variables)
            Reject(item.Value.Type == Value.ValueType.ToDo, LOCATION, $"ToDo {item.Key} hasn't been implemented");
      }

      public override bool Exists(string name) => ContainsMessage(name);

      public IStaticObject StaticObject { get; set; }

      public bool Empty { get; set; }

      public long ID { get; set; }

      public bool IsObject { get; set; }

      public override Region Public()
      {
         var region = new Region();
         foreach (var item in Variables.Where(i => isPublic(i.Key)))
            region[item.Key] = item.Value;

         return region;
      }

      public List<Block> StaticBlocks => staticBlocks;

      public override void SetVariable(string name, Value value)
      {
         base.SetVariable(name, value);
         removeAbstracts(name);
      }

      void removeAbstracts(string name)
      {
         var getter = LongToMangledPrefix("get", name);
         if (variables.ContainsKey(getter))
         {
            var gettable = variables[getter];
            if (gettable.Type == Value.ValueType.Abstract)
               variables.Remove(getter);
         }
         switch (ReadOnlys[name])
         {
            case ReadOnlyType.ReadWrite:
               var setter = LongToMangledPrefix("set", name);
               if (variables.ContainsKey(setter))
               {
                  var settable = variables[setter];
                  if (settable.Type == Value.ValueType.Abstract)
                     variables.Remove(setter);
               }
               break;
         }
      }

      protected override void setValue(string name, Value value, VisibilityType visibility, bool _override,
         bool allowNil, int index)
      {
         base.setValue(name, value, visibility, _override, allowNil, index);
         if (value is InvokeableReference)
            return;

         var builder = new CodeBuilder();
         builder.Parameter("$0");
         builder.AssignToField(name, new Variable("$0"), index);
         var lambda = builder.Lambda();
         var setterName = SetterName(name);
         var invokeableName = InvokeableName(setterName);
         State.SetInvokeable(invokeableName, lambda);
         base.setValue(setterName, new InvokeableReference(invokeableName), VisibilityType.Public, false, false, index);
         removeAbstracts(name);
      }

      public override Region ReferenceClone() => new ObjectBuildingRegion(className);

      public override Region ReferenceClone<TRegion>(TRegion target)
      {
         var objectBuildingRegion = target as ObjectBuildingRegion;
         if (objectBuildingRegion != null)
         {
            objectBuildingRegion.staticBlocks = staticBlocks;
            objectBuildingRegion.invariants = invariants;
            objectBuildingRegion.ID = ID;
            objectBuildingRegion.StaticObject = StaticObject;
            objectBuildingRegion.Empty = Empty;
            objectBuildingRegion.ID = ID;
         }

         return target;
      }
   }
}