using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Eventing.AzureServiceBus.Api
{
    public static class FolderModelStereotypeExtensions
    {
        public static AzureServiceBusFolderSettings GetAzureServiceBusFolderSettings(this FolderModel model)
        {
            var stereotype = model.GetStereotype(AzureServiceBusFolderSettings.DefinitionId);
            return stereotype != null ? new AzureServiceBusFolderSettings(stereotype) : null;
        }


        public static bool HasAzureServiceBusFolderSettings(this FolderModel model)
        {
            return model.HasStereotype(AzureServiceBusFolderSettings.DefinitionId);
        }

        public static bool TryGetAzureServiceBusFolderSettings(this FolderModel model, out AzureServiceBusFolderSettings stereotype)
        {
            if (!HasAzureServiceBusFolderSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new AzureServiceBusFolderSettings(model.GetStereotype(AzureServiceBusFolderSettings.DefinitionId));
            return true;
        }

        public class AzureServiceBusFolderSettings
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "706576a2-ddb3-4ccd-9b13-ee58147b0e6b";

            public AzureServiceBusFolderSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

        }

    }
}