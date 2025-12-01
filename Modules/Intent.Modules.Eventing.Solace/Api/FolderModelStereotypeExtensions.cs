using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Eventing.Solace.Api
{
    public static class FolderModelStereotypeExtensions
    {
        public static SolaceFolderSettings GetSolaceFolderSettings(this FolderModel model)
        {
            var stereotype = model.GetStereotype(SolaceFolderSettings.DefinitionId);
            return stereotype != null ? new SolaceFolderSettings(stereotype) : null;
        }


        public static bool HasSolaceFolderSettings(this FolderModel model)
        {
            return model.HasStereotype(SolaceFolderSettings.DefinitionId);
        }

        public static bool TryGetSolaceFolderSettings(this FolderModel model, out SolaceFolderSettings stereotype)
        {
            if (!HasSolaceFolderSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new SolaceFolderSettings(model.GetStereotype(SolaceFolderSettings.DefinitionId));
            return true;
        }

        public class SolaceFolderSettings
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "d0dc5fff-184e-421d-951f-036c0e250bec";

            public SolaceFolderSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

        }

    }
}