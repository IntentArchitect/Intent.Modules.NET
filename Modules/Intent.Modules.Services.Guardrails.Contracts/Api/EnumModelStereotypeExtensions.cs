using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Services.Guardrails.Contracts.Api
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
            public const string DefinitionId = "ea86d23f-bd74-42b8-863d-98025fb5b7a7";

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
                        case "none":
                            return AccessModifierOptionsEnum.None;
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
                public bool IsNone()
                {
                    return Value == "none";
                }
            }

            public enum AccessModifierOptionsEnum
            {
                Public,
                Internal,
                None
            }
        }

        public class FileConfig
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "f58ac38b-545d-4b87-9560-c3430ef415ed";

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