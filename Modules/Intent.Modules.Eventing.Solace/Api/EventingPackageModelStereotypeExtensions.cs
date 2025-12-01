using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Eventing.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Eventing.Solace.Api
{
    public static class EventingPackageModelStereotypeExtensions
    {
        public static SolacePackageSettings GetSolacePackageSettings(this EventingPackageModel model)
        {
            var stereotype = model.GetStereotype(SolacePackageSettings.DefinitionId);
            return stereotype != null ? new SolacePackageSettings(stereotype) : null;
        }


        public static bool HasSolacePackageSettings(this EventingPackageModel model)
        {
            return model.HasStereotype(SolacePackageSettings.DefinitionId);
        }

        public static bool TryGetSolacePackageSettings(this EventingPackageModel model, out SolacePackageSettings stereotype)
        {
            if (!HasSolacePackageSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new SolacePackageSettings(model.GetStereotype(SolacePackageSettings.DefinitionId));
            return true;
        }

        public class SolacePackageSettings
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "1edcb13a-0108-42a3-b9d0-dea95dc655b8";

            public SolacePackageSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

        }

    }
}