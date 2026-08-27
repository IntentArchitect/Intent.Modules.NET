using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Eventing.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Eventing.Wolverine.Api
{
    public static class MessageModelStereotypeExtensions
    {
        public static WolverineMessage GetWolverineMessage(this MessageModel model)
        {
            var stereotype = model.GetStereotype(WolverineMessage.DefinitionId);
            return stereotype != null ? new WolverineMessage(stereotype) : null;
        }


        public static bool HasWolverineMessage(this MessageModel model)
        {
            return model.HasStereotype(WolverineMessage.DefinitionId);
        }

        public static bool TryGetWolverineMessage(this MessageModel model, out WolverineMessage stereotype)
        {
            if (!HasWolverineMessage(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new WolverineMessage(model.GetStereotype(WolverineMessage.DefinitionId));
            return true;
        }

        public class WolverineMessage
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "292b1227-a311-4bc1-bbf5-5f0ba070b0b1";

            public WolverineMessage(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string TopicName()
            {
                return _stereotype.GetProperty<string>("Topic Name");
            }

            public string DestinationQueueName()
            {
                return _stereotype.GetProperty<string>("Destination Queue Name");
            }

        }

    }
}