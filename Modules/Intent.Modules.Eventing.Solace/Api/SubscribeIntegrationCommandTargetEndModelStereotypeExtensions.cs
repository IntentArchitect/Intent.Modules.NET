using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Services.EventInteractions;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Eventing.Solace.Api
{
    public static class SubscribeIntegrationCommandTargetEndModelStereotypeExtensions
    {
        public static Subscribing GetSubscribing(this SubscribeIntegrationCommandTargetEndModel model)
        {
            var stereotype = model.GetStereotype(Subscribing.DefinitionId);
            return stereotype != null ? new Subscribing(stereotype) : null;
        }


        public static bool HasSubscribing(this SubscribeIntegrationCommandTargetEndModel model)
        {
            return model.HasStereotype(Subscribing.DefinitionId);
        }

        public static bool TryGetSubscribing(this SubscribeIntegrationCommandTargetEndModel model, out Subscribing stereotype)
        {
            if (!HasSubscribing(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Subscribing(model.GetStereotype(Subscribing.DefinitionId));
            return true;
        }

        public class Subscribing
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "600ea351-52b9-4b32-982f-614fb5b26ed1";

            public Subscribing(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public IElement Queue()
            {
                return _stereotype.GetProperty<IElement>("Queue");
            }

            public string Topic()
            {
                return _stereotype.GetProperty<string>("Topic");
            }

        }

    }
}