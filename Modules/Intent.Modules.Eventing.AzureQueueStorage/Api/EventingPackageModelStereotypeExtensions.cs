using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Eventing.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Eventing.AzureQueueStorage.Api
{
    public static class EventingPackageModelStereotypeExtensions
    {
        public static AzureQueueStoragePackageSettings GetAzureQueueStoragePackageSettings(this EventingPackageModel model)
        {
            var stereotype = model.GetStereotype(AzureQueueStoragePackageSettings.DefinitionId);
            return stereotype != null ? new AzureQueueStoragePackageSettings(stereotype) : null;
        }


        public static bool HasAzureQueueStoragePackageSettings(this EventingPackageModel model)
        {
            return model.HasStereotype(AzureQueueStoragePackageSettings.DefinitionId);
        }

        public static bool TryGetAzureQueueStoragePackageSettings(this EventingPackageModel model, out AzureQueueStoragePackageSettings stereotype)
        {
            if (!HasAzureQueueStoragePackageSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new AzureQueueStoragePackageSettings(model.GetStereotype(AzureQueueStoragePackageSettings.DefinitionId));
            return true;
        }

        public class AzureQueueStoragePackageSettings
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "32372bfd-d61a-4782-b542-427b0f6eb7b3";

            public AzureQueueStoragePackageSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

        }

    }
}