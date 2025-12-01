using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Eventing.Kafka.Api
{
    public static class FolderModelStereotypeExtensions
    {
        public static KafkaFolderSettings GetKafkaFolderSettings(this FolderModel model)
        {
            var stereotype = model.GetStereotype(KafkaFolderSettings.DefinitionId);
            return stereotype != null ? new KafkaFolderSettings(stereotype) : null;
        }


        public static bool HasKafkaFolderSettings(this FolderModel model)
        {
            return model.HasStereotype(KafkaFolderSettings.DefinitionId);
        }

        public static bool TryGetKafkaFolderSettings(this FolderModel model, out KafkaFolderSettings stereotype)
        {
            if (!HasKafkaFolderSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new KafkaFolderSettings(model.GetStereotype(KafkaFolderSettings.DefinitionId));
            return true;
        }

        public class KafkaFolderSettings
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "762ba7ac-4039-4a56-8f8d-e0ecd59038c5";

            public KafkaFolderSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

        }

    }
}