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
    public static class SubscribeIntegrationCommandTargetEndModelStereotypeExtensions
    {
        public static CommandConsumption GetCommandConsumption(this SubscribeIntegrationCommandTargetEndModel model)
        {
            var stereotype = model.GetStereotype("Command Consumption");
            return stereotype != null ? new CommandConsumption(stereotype) : null;
        }


        public static bool HasCommandConsumption(this SubscribeIntegrationCommandTargetEndModel model)
        {
            return model.HasStereotype("Command Consumption");
        }

        public static bool TryGetCommandConsumption(this SubscribeIntegrationCommandTargetEndModel model, out CommandConsumption stereotype)
        {
            if (!HasCommandConsumption(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new CommandConsumption(model.GetStereotype("Command Consumption"));
            return true;
        }

        public class CommandConsumption
        {
            private IStereotype _stereotype;

            public CommandConsumption(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string QueueName()
            {
                return _stereotype.GetProperty<string>("Queue Name");
            }

        }

    }
}