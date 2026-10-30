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
    public static class ClassConstructorModelStereotypeExtensions
    {
        public static ConstructorConfiguration GetConstructorConfiguration(this ClassConstructorModel model)
        {
            var stereotype = model.GetStereotype(ConstructorConfiguration.DefinitionId);
            return stereotype != null ? new ConstructorConfiguration(stereotype) : null;
        }


        public static bool HasConstructorConfiguration(this ClassConstructorModel model)
        {
            return model.HasStereotype(ConstructorConfiguration.DefinitionId);
        }

        public static bool TryGetConstructorConfiguration(this ClassConstructorModel model, out ConstructorConfiguration stereotype)
        {
            if (!HasConstructorConfiguration(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new ConstructorConfiguration(model.GetStereotype(ConstructorConfiguration.DefinitionId));
            return true;
        }
        public static PrimaryConstructor GetPrimaryConstructor(this ClassConstructorModel model)
        {
            var stereotype = model.GetStereotype(PrimaryConstructor.DefinitionId);
            return stereotype != null ? new PrimaryConstructor(stereotype) : null;
        }


        public static bool HasPrimaryConstructor(this ClassConstructorModel model)
        {
            return model.HasStereotype(PrimaryConstructor.DefinitionId);
        }

        public static bool TryGetPrimaryConstructor(this ClassConstructorModel model, out PrimaryConstructor stereotype)
        {
            if (!HasPrimaryConstructor(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new PrimaryConstructor(model.GetStereotype(PrimaryConstructor.DefinitionId));
            return true;
        }

        public class ConstructorConfiguration
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "2189ac42-f723-4663-9673-80ad2cdd9f51";

            public ConstructorConfiguration(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public AccessModifierOptions AccessModifier()
            {
                return new AccessModifierOptions(_stereotype.GetProperty<string>("Access Modifier"));
            }

            public class AccessModifierOptions
            {
                public readonly string Value;

                public AccessModifierOptions(string value)
                {
                    Value = value;
                }

                public AccessModifierOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "public":
                            return AccessModifierOptionsEnum.Public;
                        case "protected":
                            return AccessModifierOptionsEnum.Protected;
                        case "internal":
                            return AccessModifierOptionsEnum.Internal;
                        case "private":
                            return AccessModifierOptionsEnum.Private;
                        case "protected internal":
                            return AccessModifierOptionsEnum.ProtectedInternal;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsPublic()
                {
                    return Value == "public";
                }
                public bool IsProtected()
                {
                    return Value == "protected";
                }
                public bool IsInternal()
                {
                    return Value == "internal";
                }
                public bool IsPrivate()
                {
                    return Value == "private";
                }
                public bool IsProtectedInternal()
                {
                    return Value == "protected internal";
                }
            }

            public enum AccessModifierOptionsEnum
            {
                Public,
                Protected,
                Internal,
                Private,
                ProtectedInternal
            }
        }

        public class PrimaryConstructor
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "d95d7c59-fe11-4f0a-84a9-d17a8c097cc1";

            public PrimaryConstructor(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

        }

    }
}