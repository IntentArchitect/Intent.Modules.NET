using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Eventing.MassTransit.RequestResponse.Api
{
    public static class CommandModelStereotypeExtensions
    {
        public static MessageRequestEndpoint GetMessageRequestEndpoint(this CommandModel model)
        {
            var stereotype = model.GetStereotype("5150384d-18a3-4c7e-a716-599e6658abde");
            return stereotype != null ? new MessageRequestEndpoint(stereotype) : null;
        }


        public static bool HasMessageRequestEndpoint(this CommandModel model)
        {
            return model.HasStereotype("5150384d-18a3-4c7e-a716-599e6658abde");
        }

        public static bool TryGetMessageRequestEndpoint(this CommandModel model, out MessageRequestEndpoint stereotype)
        {
            if (!HasMessageRequestEndpoint(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new MessageRequestEndpoint(model.GetStereotype("5150384d-18a3-4c7e-a716-599e6658abde"));
            return true;
        }

        public class MessageRequestEndpoint
        {
            private IStereotype _stereotype;

            public MessageRequestEndpoint(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

        }

    }
}