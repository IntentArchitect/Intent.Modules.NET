using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Domain.Guardrails.Contracts.Api
{
    public static class EnumModelStereotypeExtensions
    {
        public static EnumConfiguration GetEnumConfiguration(this EnumModel model)
        {
            var stereotype = model.GetStereotype(EnumConfiguration.DefinitionId);
            return stereotype != null ? new EnumConfiguration(stereotype) : null;
        }


        public static bool HasEnumConfiguration(this EnumModel model)
        {
            return model.HasStereotype(EnumConfiguration.DefinitionId);
        }

        public static bool TryGetEnumConfiguration(this EnumModel model, out EnumConfiguration stereotype)
        {
            if (!HasEnumConfiguration(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new EnumConfiguration(model.GetStereotype(EnumConfiguration.DefinitionId));
            return true;
        }
        public static FileConfig GetFileConfig(this EnumModel model)
        {
            var stereotype = model.GetStereotype(FileConfig.DefinitionId);
            return stereotype != null ? new FileConfig(stereotype) : null;
        }


        public static bool HasFileConfig(this EnumModel model)
        {
            return model.HasStereotype(FileConfig.DefinitionId);
        }

        public static bool TryGetFileConfig(this EnumModel model, out FileConfig stereotype)
        {
            if (!HasFileConfig(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new FileConfig(model.GetStereotype(FileConfig.DefinitionId));
            return true;
        }

        public class EnumConfiguration
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "5f9ff631-18cd-4320-b33c-7024de5b7914";

            public EnumConfiguration(IStereotype stereotype)
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
                        case "internal":
                            return AccessModifierOptionsEnum.Internal;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsPublic()
                {
                    return Value == "public";
                }
                public bool IsInternal()
                {
                    return Value == "internal";
                }
            }

            public enum AccessModifierOptionsEnum
            {
                Public,
                Internal
            }
        }

        public class FileConfig
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "88722cfc-8f4e-4cc2-96f5-3635f5b520d1";

            public FileConfig(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string Filename()
            {
                return _stereotype.GetProperty<string>("Filename");
            }

            public string Namespace()
            {
                return _stereotype.GetProperty<string>("Namespace");
            }

            public bool NoNamespace()
            {
                return _stereotype.GetProperty<bool>("No Namespace");
            }

        }

    }
}