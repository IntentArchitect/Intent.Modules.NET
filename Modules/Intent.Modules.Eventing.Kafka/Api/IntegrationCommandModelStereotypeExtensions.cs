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
        public static KafkaMessageSettings GetKafkaMessageSettings(this IntegrationCommandModel model)
        {
            var stereotype = model.GetStereotype(KafkaMessageSettings.DefinitionId);
            return stereotype != null ? new KafkaMessageSettings(stereotype) : null;
        }


        public static bool HasKafkaMessageSettings(this IntegrationCommandModel model)
        {
            return model.HasStereotype(KafkaMessageSettings.DefinitionId);
        }

        public static bool TryGetKafkaMessageSettings(this IntegrationCommandModel model, out KafkaMessageSettings stereotype)
        {
            if (!HasKafkaMessageSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new KafkaMessageSettings(model.GetStereotype(KafkaMessageSettings.DefinitionId));
            return true;
        }

        public class KafkaMessageSettings
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "f18ed242-c439-4b46-834c-bc2947731486";

            public KafkaMessageSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

        }

    }
}