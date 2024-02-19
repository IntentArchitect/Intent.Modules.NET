using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Services.EventInteractions;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Eventing.MassTransit.Api
{
    public static class SendIntegrationCommandTargetEndModelStereotypeExtensions
    {
        public static CommandDistribution GetCommandDistribution(this SendIntegrationCommandTargetEndModel model)
        {
            var stereotype = model.GetStereotype("5cae1c25-cc30-4af8-8955-30af001c981d");
            return stereotype != null ? new CommandDistribution(stereotype) : null;
        }


        public static bool HasCommandDistribution(this SendIntegrationCommandTargetEndModel model)
        {
            return model.HasStereotype("5cae1c25-cc30-4af8-8955-30af001c981d");
        }

        public static bool TryGetCommandDistribution(this SendIntegrationCommandTargetEndModel model, out CommandDistribution stereotype)
        {
            if (!HasCommandDistribution(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new CommandDistribution(model.GetStereotype("5cae1c25-cc30-4af8-8955-30af001c981d"));
            return true;
        }

        public class CommandDistribution
        {
            private IStereotype _stereotype;

            public CommandDistribution(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string DestinationQueueName()
            {
                return _stereotype.GetProperty<string>("Destination Queue Name");
            }

        }

    }
}