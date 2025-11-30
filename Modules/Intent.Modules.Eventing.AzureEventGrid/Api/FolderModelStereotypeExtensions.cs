using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Eventing.AzureEventGrid.Api
{
    public static class FolderModelStereotypeExtensions
    {
        public static AzureEventGridFolderSettings GetAzureEventGridFolderSettings(this FolderModel model)
        {
            var stereotype = model.GetStereotype(AzureEventGridFolderSettings.DefinitionId);
            return stereotype != null ? new AzureEventGridFolderSettings(stereotype) : null;
        }


        public static bool HasAzureEventGridFolderSettings(this FolderModel model)
        {
            return model.HasStereotype(AzureEventGridFolderSettings.DefinitionId);
        }

        public static bool TryGetAzureEventGridFolderSettings(this FolderModel model, out AzureEventGridFolderSettings stereotype)
        {
            if (!HasAzureEventGridFolderSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new AzureEventGridFolderSettings(model.GetStereotype(AzureEventGridFolderSettings.DefinitionId));
            return true;
        }

        public class AzureEventGridFolderSettings
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "84e5a563-953d-43f5-b2ca-a24ce3104e0b";

            public AzureEventGridFolderSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

        }

    }
}