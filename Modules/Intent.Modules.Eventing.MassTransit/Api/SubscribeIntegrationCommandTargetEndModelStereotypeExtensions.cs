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
        public static MessageConsumption GetMessageConsumption(this SubscribeIntegrationCommandTargetEndModel model)
        {
            var stereotype = model.GetStereotype("Message Consumption");
            return stereotype != null ? new MessageConsumption(stereotype) : null;
        }


        public static bool HasMessageConsumption(this SubscribeIntegrationCommandTargetEndModel model)
        {
            return model.HasStereotype("Message Consumption");
        }

        public static bool TryGetMessageConsumption(this SubscribeIntegrationCommandTargetEndModel model, out MessageConsumption stereotype)
        {
            if (!HasMessageConsumption(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new MessageConsumption(model.GetStereotype("Message Consumption"));
            return true;
        }

        public class MessageConsumption
        {
            private IStereotype _stereotype;

            public MessageConsumption(IStereotype stereotype)
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