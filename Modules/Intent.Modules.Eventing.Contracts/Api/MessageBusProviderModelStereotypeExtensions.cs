using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Eventing.Contracts.Api
{
    public static class MessageBusProviderModelStereotypeExtensions
    {
        public static MessageBusProviderSettings GetMessageBusProviderSettings(this MessageBusProviderModel model)
        {
            var stereotype = model.GetStereotype(MessageBusProviderSettings.DefinitionId);
            return stereotype != null ? new MessageBusProviderSettings(stereotype) : null;
        }


        public static bool HasMessageBusProviderSettings(this MessageBusProviderModel model)
        {
            return model.HasStereotype(MessageBusProviderSettings.DefinitionId);
        }

        public static bool TryGetMessageBusProviderSettings(this MessageBusProviderModel model, out MessageBusProviderSettings stereotype)
        {
            if (!HasMessageBusProviderSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new MessageBusProviderSettings(model.GetStereotype(MessageBusProviderSettings.DefinitionId));
            return true;
        }

        public class MessageBusProviderSettings
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "bf50a93c-cf7d-414d-9d30-34f31bb696ad";

            public MessageBusProviderSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public IStereotypeDefinition[] ApplicableStereotypes()
            {
                return _stereotype.GetProperty<IStereotypeDefinition[]>("Applicable Stereotypes");
            }

        }

    }
}