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
    public static class SendIntegrationCommandTargetEndModelStereotypeExtensions
    {
        public static MessageDistribuion GetMessageDistribuion(this SendIntegrationCommandTargetEndModel model)
        {
            var stereotype = model.GetStereotype("Message Distribuion");
            return stereotype != null ? new MessageDistribuion(stereotype) : null;
        }


        public static bool HasMessageDistribuion(this SendIntegrationCommandTargetEndModel model)
        {
            return model.HasStereotype("Message Distribuion");
        }

        public static bool TryGetMessageDistribuion(this SendIntegrationCommandTargetEndModel model, out MessageDistribuion stereotype)
        {
            if (!HasMessageDistribuion(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new MessageDistribuion(model.GetStereotype("Message Distribuion"));
            return true;
        }

        public class MessageDistribuion
        {
            private IStereotype _stereotype;

            public MessageDistribuion(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string SendAddress()
            {
                return _stereotype.GetProperty<string>("Send Address");
            }

            public string AppSettingName()
            {
                return _stereotype.GetProperty<string>("AppSetting Name");
            }

        }

    }
}