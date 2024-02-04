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
            var stereotype = model.GetStereotype("Command Distribution");
            return stereotype != null ? new CommandDistribution(stereotype) : null;
        }


        public static bool HasCommandDistribution(this SendIntegrationCommandTargetEndModel model)
        {
            return model.HasStereotype("Command Distribution");
        }

        public static bool TryGetCommandDistribution(this SendIntegrationCommandTargetEndModel model, out CommandDistribution stereotype)
        {
            if (!HasCommandDistribution(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new CommandDistribution(model.GetStereotype("Command Distribution"));
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