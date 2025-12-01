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
    public static class EventingPackageModelStereotypeExtensions
    {
        public static KafkaPackageSettings GetKafkaPackageSettings(this EventingPackageModel model)
        {
            var stereotype = model.GetStereotype(KafkaPackageSettings.DefinitionId);
            return stereotype != null ? new KafkaPackageSettings(stereotype) : null;
        }


        public static bool HasKafkaPackageSettings(this EventingPackageModel model)
        {
            return model.HasStereotype(KafkaPackageSettings.DefinitionId);
        }

        public static bool TryGetKafkaPackageSettings(this EventingPackageModel model, out KafkaPackageSettings stereotype)
        {
            if (!HasKafkaPackageSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new KafkaPackageSettings(model.GetStereotype(KafkaPackageSettings.DefinitionId));
            return true;
        }

        public class KafkaPackageSettings
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "a08a5b95-24a9-46dd-b52a-5faed1db9955";

            public KafkaPackageSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

        }

    }
}