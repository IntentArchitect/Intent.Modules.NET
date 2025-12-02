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
        public static MassTransitMessage GetMassTransitMessage(this IntegrationCommandModel model)
        {
            var stereotype = model.GetStereotype(MassTransitMessage.DefinitionId);
            return stereotype != null ? new MassTransitMessage(stereotype) : null;
        }


        public static bool HasMassTransitMessage(this IntegrationCommandModel model)
        {
            return model.HasStereotype(MassTransitMessage.DefinitionId);
        }

        public static bool TryGetMassTransitMessage(this IntegrationCommandModel model, out MassTransitMessage stereotype)
        {
            if (!HasMassTransitMessage(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new MassTransitMessage(model.GetStereotype(MassTransitMessage.DefinitionId));
            return true;
        }

        public class MassTransitMessage
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "19664e24-b935-4822-ac61-26d47488be42";

            public MassTransitMessage(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

        }

    }
}