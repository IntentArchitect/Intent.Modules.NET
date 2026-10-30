using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Services.Guardrails.Contracts.Api
{
    public static class ParameterModelStereotypeExtensions
    {
        public static CollectionType GetCollectionType(this ParameterModel model)
        {
            var stereotype = model.GetStereotype(CollectionType.DefinitionId);
            return stereotype != null ? new CollectionType(stereotype) : null;
        }


        public static bool HasCollectionType(this ParameterModel model)
        {
            return model.HasStereotype(CollectionType.DefinitionId);
        }

        public static bool TryGetCollectionType(this ParameterModel model, out CollectionType stereotype)
        {
            if (!HasCollectionType(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new CollectionType(model.GetStereotype(CollectionType.DefinitionId));
            return true;
        }

        public static ParameterModifier GetParameterModifier(this ParameterModel model)
        {
            var stereotype = model.GetStereotype(ParameterModifier.DefinitionId);
            return stereotype != null ? new ParameterModifier(stereotype) : null;
        }


        public static bool HasParameterModifier(this ParameterModel model)
        {
            return model.HasStereotype(ParameterModifier.DefinitionId);
        }

        public static bool TryGetParameterModifier(this ParameterModel model, out ParameterModifier stereotype)
        {
            if (!HasParameterModifier(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new ParameterModifier(model.GetStereotype(ParameterModifier.DefinitionId));
            return true;
        }

        public class CollectionType
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "2550e9f2-2d5c-4a98-b6a9-8348f86f06b2";

            public CollectionType(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public TypeOptions Type()
            {
                return new TypeOptions(_stereotype.GetProperty<string>("Type"));
            }

            public string CustomFormat()
            {
                return _stereotype.GetProperty<string>("Custom Format");
            }

            public class TypeOptions
            {
                public readonly string Value;

                public TypeOptions(string value)
                {
                    Value = value;
                }

                public TypeOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "IEnumerable":
                            return TypeOptionsEnum.IEnumerable;
                        case "ICollection":
                            return TypeOptionsEnum.ICollection;
                        case "IList":
                            return TypeOptionsEnum.IList;
                        case "IReadOnlyCollection":
                            return TypeOptionsEnum.IReadOnlyCollection;
                        case "IReadOnlyList":
                            return TypeOptionsEnum.IReadOnlyList;
                        case "List":
                            return TypeOptionsEnum.List;
                        case "HashSet":
                            return TypeOptionsEnum.HashSet;
                        case "ISet":
                            return TypeOptionsEnum.ISet;
                        case "IQueryable":
                            return TypeOptionsEnum.IQueryable;
                        case "Array":
                            return TypeOptionsEnum.Array;
                        case "Custom":
                            return TypeOptionsEnum.Custom;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsIEnumerable()
                {
                    return Value == "IEnumerable";
                }
                public bool IsICollection()
                {
                    return Value == "ICollection";
                }
                public bool IsIList()
                {
                    return Value == "IList";
                }
                public bool IsIReadOnlyCollection()
                {
                    return Value == "IReadOnlyCollection";
                }
                public bool IsIReadOnlyList()
                {
                    return Value == "IReadOnlyList";
                }
                public bool IsList()
                {
                    return Value == "List";
                }
                public bool IsHashSet()
                {
                    return Value == "HashSet";
                }
                public bool IsISet()
                {
                    return Value == "ISet";
                }
                public bool IsIQueryable()
                {
                    return Value == "IQueryable";
                }
                public bool IsArray()
                {
                    return Value == "Array";
                }
                public bool IsCustom()
                {
                    return Value == "Custom";
                }
            }

            public enum TypeOptionsEnum
            {
                IEnumerable,
                ICollection,
                IList,
                IReadOnlyCollection,
                IReadOnlyList,
                List,
                HashSet,
                ISet,
                IQueryable,
                Array,
                Custom
            }
        }

        public class ParameterModifier
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "2a2fd52d-38d0-43d9-bd47-6118706b55c6";

            public ParameterModifier(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public ModifierOptions Modifier()
            {
                return new ModifierOptions(_stereotype.GetProperty<string>("Modifier"));
            }

            public class ModifierOptions
            {
                public readonly string Value;

                public ModifierOptions(string value)
                {
                    Value = value;
                }

                public ModifierOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "ref":
                            return ModifierOptionsEnum.Ref;
                        case "out":
                            return ModifierOptionsEnum.Out;
                        case "in":
                            return ModifierOptionsEnum.In;
                        case "ref readonly":
                            return ModifierOptionsEnum.RefReadonly;
                        case "params":
                            return ModifierOptionsEnum.Params;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsRef()
                {
                    return Value == "ref";
                }
                public bool IsOut()
                {
                    return Value == "out";
                }
                public bool IsIn()
                {
                    return Value == "in";
                }
                public bool IsRefReadonly()
                {
                    return Value == "ref readonly";
                }
                public bool IsParams()
                {
                    return Value == "params";
                }
            }

            public enum ModifierOptionsEnum
            {
                Ref,
                Out,
                In,
                RefReadonly,
                Params
            }
        }

    }
}