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
        public static MessageTriggered GetMessageTriggered(this CommandModel model)
        {
            var stereotype = model.GetStereotype(MessageTriggered.DefinitionId);
            return stereotype != null ? new MessageTriggered(stereotype) : null;
        }


        public static bool HasMessageTriggered(this CommandModel model)
        {
            return model.HasStereotype(MessageTriggered.DefinitionId);
        }

        public static bool TryGetMessageTriggered(this CommandModel model, out MessageTriggered stereotype)
        {
            if (!HasMessageTriggered(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new MessageTriggered(model.GetStereotype(MessageTriggered.DefinitionId));
            return true;
        }

        public class MessageTriggered
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "5150384d-18a3-4c7e-a716-599e6658abde";

            public MessageTriggered(IStereotype stereotype)
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