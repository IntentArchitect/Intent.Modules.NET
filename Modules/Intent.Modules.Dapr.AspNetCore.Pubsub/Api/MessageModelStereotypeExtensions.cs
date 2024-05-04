using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Eventing.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Dapr.AspNetCore.Pubsub.Api
{
    public static class MessageModelStereotypeExtensions
    {
        public static DaprSettings GetDaprSettings(this MessageModel model)
        {
            var stereotype = model.GetStereotype("ec96e452-9084-49bb-a883-aa42eb327fe7");
            return stereotype != null ? new DaprSettings(stereotype) : null;
        }


        public static bool HasDaprSettings(this MessageModel model)
        {
            return model.HasStereotype("ec96e452-9084-49bb-a883-aa42eb327fe7");
        }

        public static bool TryGetDaprSettings(this MessageModel model, out DaprSettings stereotype)
        {
            if (!HasDaprSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new DaprSettings(model.GetStereotype("ec96e452-9084-49bb-a883-aa42eb327fe7"));
            return true;
        }

        public class DaprSettings
        {
            private IStereotype _stereotype;

            public DaprSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string TopicName()
            {
                return _stereotype.GetProperty<string>("Topic Name");
            }

            public string PubsubName()
            {
                return _stereotype.GetProperty<string>("Pubsub Name");
            }

        }

    }
}