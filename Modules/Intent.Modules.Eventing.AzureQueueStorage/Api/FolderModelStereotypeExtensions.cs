using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Eventing.AzureQueueStorage.Api
{
    public static class FolderModelStereotypeExtensions
    {
        public static AzureQueueStorageFolderSettings GetAzureQueueStorageFolderSettings(this FolderModel model)
        {
            var stereotype = model.GetStereotype(AzureQueueStorageFolderSettings.DefinitionId);
            return stereotype != null ? new AzureQueueStorageFolderSettings(stereotype) : null;
        }


        public static bool HasAzureQueueStorageFolderSettings(this FolderModel model)
        {
            return model.HasStereotype(AzureQueueStorageFolderSettings.DefinitionId);
        }

        public static bool TryGetAzureQueueStorageFolderSettings(this FolderModel model, out AzureQueueStorageFolderSettings stereotype)
        {
            if (!HasAzureQueueStorageFolderSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new AzureQueueStorageFolderSettings(model.GetStereotype(AzureQueueStorageFolderSettings.DefinitionId));
            return true;
        }

        public class AzureQueueStorageFolderSettings
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "a9fcfa58-3b1d-4126-8bdc-26d441f9880c";

            public AzureQueueStorageFolderSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

        }

    }
}