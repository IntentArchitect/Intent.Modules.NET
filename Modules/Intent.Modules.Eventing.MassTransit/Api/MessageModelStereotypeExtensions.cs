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
        public static MessageSettings GetMessageSettings(this MessageModel model)
        {
            var stereotype = model.GetStereotype("Message Settings");
            return stereotype != null ? new MessageSettings(stereotype) : null;
        }


        public static bool HasMessageSettings(this MessageModel model)
        {
            return model.HasStereotype("Message Settings");
        }

        public static bool TryGetMessageSettings(this MessageModel model, out MessageSettings stereotype)
        {
            if (!HasMessageSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new MessageSettings(model.GetStereotype("Message Settings"));
            return true;
        }

        public class MessageSettings
        {
            private IStereotype _stereotype;

            public MessageSettings(IStereotype stereotype)
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