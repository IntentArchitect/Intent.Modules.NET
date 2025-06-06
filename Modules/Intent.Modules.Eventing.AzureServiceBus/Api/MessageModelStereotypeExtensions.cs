using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Eventing.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Eventing.AzureServiceBus.Api
{
    public static class MessageModelStereotypeExtensions
    {
        public static AzureServiceBus GetAzureServiceBus(this MessageModel model)
        {
            var stereotype = model.GetStereotype(AzureServiceBus.DefinitionId);
            return stereotype != null ? new AzureServiceBus(stereotype) : null;
        }


        public static bool HasAzureServiceBus(this MessageModel model)
        {
            return model.HasStereotype(AzureServiceBus.DefinitionId);
        }

        public static bool TryGetAzureServiceBus(this MessageModel model, out AzureServiceBus stereotype)
        {
            if (!HasAzureServiceBus(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new AzureServiceBus(model.GetStereotype(AzureServiceBus.DefinitionId));
            return true;
        }

        public class AzureServiceBus
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "1f60bd15-005b-4184-8c12-c44c20158001";

            public AzureServiceBus(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public TypeOptions Type()
            {
                return new TypeOptions(_stereotype.GetProperty<string>("Type"));
            }

            public string QueueName()
            {
                return _stereotype.GetProperty<string>("Queue Name");
            }

            public string TopicName()
            {
                return _stereotype.GetProperty<string>("Topic Name");
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
                        case "Queue":
                            return TypeOptionsEnum.Queue;
                        case "Topic":
                            return TypeOptionsEnum.Topic;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsQueue()
                {
                    return Value == "Queue";
                }
                public bool IsTopic()
                {
                    return Value == "Topic";
                }
            }

            public enum TypeOptionsEnum
            {
                Queue,
                Topic
            }
        }
    }
}