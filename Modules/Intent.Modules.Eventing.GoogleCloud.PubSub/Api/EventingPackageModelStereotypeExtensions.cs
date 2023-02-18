using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Eventing.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Eventing.GoogleCloud.PubSub.Api
{
    public static class EventingPackageModelStereotypeExtensions
    {
        public static GoogleCloudSettings GetGoogleCloudSettings(this EventingPackageModel model)
        {
            var stereotype = model.GetStereotype("Google Cloud Settings");
            return stereotype != null ? new GoogleCloudSettings(stereotype) : null;
        }


        public static bool HasGoogleCloudSettings(this EventingPackageModel model)
        {
            return model.HasStereotype("Google Cloud Settings");
        }

        public static bool TryGetGoogleCloudSettings(this EventingPackageModel model, out GoogleCloudSettings stereotype)
        {
            if (!HasGoogleCloudSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new GoogleCloudSettings(model.GetStereotype("Google Cloud Settings"));
            return true;
        }

        public class GoogleCloudSettings
        {
            private IStereotype _stereotype;

            public GoogleCloudSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string TopicId()
            {
                return _stereotype.GetProperty<string>("Topic Id");
            }

        }

    }
}