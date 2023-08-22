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
    public static class MessageModelStereotypeExtensions
    {
        public static MessageTopologySettings GetMessageTopologySettings(this MessageModel model)
        {
            var stereotype = model.GetStereotype("Message Topology Settings");
            return stereotype != null ? new MessageTopologySettings(stereotype) : null;
        }


        public static bool HasMessageTopologySettings(this MessageModel model)
        {
            return model.HasStereotype("Message Topology Settings");
        }

        public static bool TryGetMessageTopologySettings(this MessageModel model, out MessageTopologySettings stereotype)
        {
            if (!HasMessageTopologySettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new MessageTopologySettings(model.GetStereotype("Message Topology Settings"));
            return true;
        }

        public class MessageTopologySettings
        {
            private IStereotype _stereotype;

            public MessageTopologySettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string EntityName()
            {
                return _stereotype.GetProperty<string>("Entity Name");
            }

        }

    }
}