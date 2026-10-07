using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Domain.Guardrails.Contracts.Api
{
    public static class AssociationTargetEndModelStereotypeExtensions
    {
        public static CollectionType GetCollectionType(this AssociationTargetEndModel model)
        {
            var stereotype = model.GetStereotype(CollectionType.DefinitionId);
            return stereotype != null ? new CollectionType(stereotype) : null;
        }


        public static bool HasCollectionType(this AssociationTargetEndModel model)
        {
            return model.HasStereotype(CollectionType.DefinitionId);
        }

        public static bool TryGetCollectionType(this AssociationTargetEndModel model, out CollectionType stereotype)
        {
            if (!HasCollectionType(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new CollectionType(model.GetStereotype(CollectionType.DefinitionId));
            return true;
        }

        public class CollectionType
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "04f4115f-35e9-4b60-b8c7-44ce30079d81";

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

    }
}