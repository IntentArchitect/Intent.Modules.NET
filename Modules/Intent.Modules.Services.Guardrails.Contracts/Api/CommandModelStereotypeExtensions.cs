using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Services.Guardrails.Contracts.Api
{
    public static class CommandModelStereotypeExtensions
    {
        public static TypeConfiguration GetTypeConfiguration(this CommandModel model)
        {
            var stereotype = model.GetStereotype(TypeConfiguration.DefinitionId);
            return stereotype != null ? new TypeConfiguration(stereotype) : null;
        }


        public static bool HasTypeConfiguration(this CommandModel model)
        {
            return model.HasStereotype(TypeConfiguration.DefinitionId);
        }

        public static bool TryGetTypeConfiguration(this CommandModel model, out TypeConfiguration stereotype)
        {
            if (!HasTypeConfiguration(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new TypeConfiguration(model.GetStereotype(TypeConfiguration.DefinitionId));
            return true;
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
            }

            public enum TypeOptionsEnum
            {
                Class,
                Record
            }
        }

    }
}