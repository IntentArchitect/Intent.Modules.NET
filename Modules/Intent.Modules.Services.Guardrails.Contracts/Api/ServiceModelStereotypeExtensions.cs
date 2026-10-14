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
    public static class ServiceModelStereotypeExtensions
    {
        public static FileConfig GetFileConfig(this ServiceModel model)
        {
            var stereotype = model.GetStereotype(FileConfig.DefinitionId);
            return stereotype != null ? new FileConfig(stereotype) : null;
        }


        public static bool HasFileConfig(this ServiceModel model)
        {
            return model.HasStereotype(FileConfig.DefinitionId);
        }

        public static bool TryGetFileConfig(this ServiceModel model, out FileConfig stereotype)
        {
            if (!HasFileConfig(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new FileConfig(model.GetStereotype(FileConfig.DefinitionId));
            return true;
        }
        public static TypeConfiguration GetTypeConfiguration(this ServiceModel model)
        {
            var stereotype = model.GetStereotype(TypeConfiguration.DefinitionId);
            return stereotype != null ? new TypeConfiguration(stereotype) : null;
        }


        public static bool HasTypeConfiguration(this ServiceModel model)
        {
            return model.HasStereotype(TypeConfiguration.DefinitionId);
        }

        public static bool TryGetTypeConfiguration(this ServiceModel model, out TypeConfiguration stereotype)
        {
            if (!HasTypeConfiguration(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new TypeConfiguration(model.GetStereotype(TypeConfiguration.DefinitionId));
            return true;
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

        public class TypeConfiguration
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "6bbf9e89-c521-4122-aa53-3fc7786f1157";

            public TypeConfiguration(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public TypeOptions Type()
            {
                return new TypeOptions(_stereotype.GetProperty<string>("Type"));
            }

            public string Modifiers()
            {
                return _stereotype.GetProperty<string>("Modifiers");
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
                        case "class":
                            return TypeOptionsEnum.Class;
                        case "record":
                            return TypeOptionsEnum.Record;
                        case "positional record":
                            return TypeOptionsEnum.PositionalRecord;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsClass()
                {
                    return Value == "class";
                }
                public bool IsRecord()
                {
                    return Value == "record";
                }
                public bool IsPositionalRecord()
                {
                    return Value == "positional record";
                }
            }

            public enum TypeOptionsEnum
            {
                Class,
                Record,
                PositionalRecord
            }
        }

    }
}