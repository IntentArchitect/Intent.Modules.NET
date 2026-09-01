using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Services.EventInteractions;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Eventing.Wolverine.Api
{
    public static class SubscribeIntegrationEventTargetEndModelStereotypeExtensions
    {
        public static WolverineSubscription GetWolverineSubscription(this SubscribeIntegrationEventTargetEndModel model)
        {
            var stereotype = model.GetStereotype(WolverineSubscription.DefinitionId);
            return stereotype != null ? new WolverineSubscription(stereotype) : null;
        }


        public static bool HasWolverineSubscription(this SubscribeIntegrationEventTargetEndModel model)
        {
            return model.HasStereotype(WolverineSubscription.DefinitionId);
        }

        public static bool TryGetWolverineSubscription(this SubscribeIntegrationEventTargetEndModel model, out WolverineSubscription stereotype)
        {
            if (!HasWolverineSubscription(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new WolverineSubscription(model.GetStereotype(WolverineSubscription.DefinitionId));
            return true;
        }

        public class WolverineSubscription
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "d87eb941-51a7-4ac2-99d1-3516fa6c58b2";

            public WolverineSubscription(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string SubscriberQueueName()
            {
                return _stereotype.GetProperty<string>("Subscriber Queue Name");
            }

        }

    }
}