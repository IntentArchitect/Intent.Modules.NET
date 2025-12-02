using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Eventing.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Eventing.Kafka.Api
{
    public static class IntegrationCommandModelStereotypeExtensions
    {
        public static KafkaMessage GetKafkaMessage(this IntegrationCommandModel model)
        {
            var stereotype = model.GetStereotype(KafkaMessage.DefinitionId);
            return stereotype != null ? new KafkaMessage(stereotype) : null;
        }


        public static bool HasKafkaMessage(this IntegrationCommandModel model)
        {
            return model.HasStereotype(KafkaMessage.DefinitionId);
        }

        public static bool TryGetKafkaMessage(this IntegrationCommandModel model, out KafkaMessage stereotype)
        {
            if (!HasKafkaMessage(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new KafkaMessage(model.GetStereotype(KafkaMessage.DefinitionId));
            return true;
        }

        public class KafkaMessage
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "f18ed242-c439-4b46-834c-bc2947731486";

            public KafkaMessage(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

        }

    }
}