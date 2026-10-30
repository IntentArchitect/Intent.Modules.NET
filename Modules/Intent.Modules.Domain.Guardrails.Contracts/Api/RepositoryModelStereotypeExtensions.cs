using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Repositories.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Domain.Guardrails.Contracts.Api
{
    public static class RepositoryModelStereotypeExtensions
    {
        public static FileConfig GetFileConfig(this RepositoryModel model)
        {
            var stereotype = model.GetStereotype(FileConfig.DefinitionId);
            return stereotype != null ? new FileConfig(stereotype) : null;
        }


        public static bool HasFileConfig(this RepositoryModel model)
        {
            return model.HasStereotype(FileConfig.DefinitionId);
        }

        public static bool TryGetFileConfig(this RepositoryModel model, out FileConfig stereotype)
        {
            if (!HasFileConfig(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new FileConfig(model.GetStereotype(FileConfig.DefinitionId));
            return true;
        }
        public static TypeConfiguration GetTypeConfiguration(this RepositoryModel model)
        {
            var stereotype = model.GetStereotype(TypeConfiguration.DefinitionId);
            return stereotype != null ? new TypeConfiguration(stereotype) : null;
        }


        public static bool HasTypeConfiguration(this RepositoryModel model)
        {
            return model.HasStereotype(TypeConfiguration.DefinitionId);
        }

        public static bool TryGetTypeConfiguration(this RepositoryModel model, out TypeConfiguration stereotype)
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

        public class TypeConfiguration
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "395ff369-19ec-4616-84cb-9be1670dac4a";

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