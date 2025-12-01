using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Eventing.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Eventing.MassTransit.Api
{
    public static class IntegrationCommandModelStereotypeExtensions
    {
        public static MassTransitMessageSettings GetMassTransitMessageSettings(this IntegrationCommandModel model)
        {
            var stereotype = model.GetStereotype(MassTransitMessageSettings.DefinitionId);
            return stereotype != null ? new MassTransitMessageSettings(stereotype) : null;
        }


        public static bool HasMassTransitMessageSettings(this IntegrationCommandModel model)
        {
            return model.HasStereotype(MassTransitMessageSettings.DefinitionId);
        }

        public static bool TryGetMassTransitMessageSettings(this IntegrationCommandModel model, out MassTransitMessageSettings stereotype)
        {
            if (!HasMassTransitMessageSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new MassTransitMessageSettings(model.GetStereotype(MassTransitMessageSettings.DefinitionId));
            return true;
        }

        public class MassTransitMessageSettings
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "fbe53252-9913-453c-b734-73b4e2dfdb46";

            public MassTransitMessageSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

        }

    }
}