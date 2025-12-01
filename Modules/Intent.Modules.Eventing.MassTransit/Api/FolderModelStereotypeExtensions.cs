using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Eventing.MassTransit.Api
{
    public static class FolderModelStereotypeExtensions
    {
        public static MassTransitFolderSettings GetMassTransitFolderSettings(this FolderModel model)
        {
            var stereotype = model.GetStereotype(MassTransitFolderSettings.DefinitionId);
            return stereotype != null ? new MassTransitFolderSettings(stereotype) : null;
        }


        public static bool HasMassTransitFolderSettings(this FolderModel model)
        {
            return model.HasStereotype(MassTransitFolderSettings.DefinitionId);
        }

        public static bool TryGetMassTransitFolderSettings(this FolderModel model, out MassTransitFolderSettings stereotype)
        {
            if (!HasMassTransitFolderSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new MassTransitFolderSettings(model.GetStereotype(MassTransitFolderSettings.DefinitionId));
            return true;
        }

        public class MassTransitFolderSettings
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "32eb7ec0-1c6f-42ae-ab3f-4a71d8882ad5";

            public MassTransitFolderSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

        }

    }
}