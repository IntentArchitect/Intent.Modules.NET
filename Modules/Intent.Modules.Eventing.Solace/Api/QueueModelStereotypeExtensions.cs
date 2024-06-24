using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Eventing.Solace.Api
{
    public static class QueueModelStereotypeExtensions
    {
        public static QueueConfig GetQueueConfig(this QueueModel model)
        {
            var stereotype = model.GetStereotype("6b1a00d5-377b-4e0d-87d3-8c2ad5c70c5d");
            return stereotype != null ? new QueueConfig(stereotype) : null;
        }


        public static bool HasQueueConfig(this QueueModel model)
        {
            return model.HasStereotype("6b1a00d5-377b-4e0d-87d3-8c2ad5c70c5d");
        }

        public static bool TryGetQueueConfig(this QueueModel model, out QueueConfig stereotype)
        {
            if (!HasQueueConfig(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new QueueConfig(model.GetStereotype("6b1a00d5-377b-4e0d-87d3-8c2ad5c70c5d"));
            return true;
        }

        public class QueueConfig
        {
            private IStereotype _stereotype;

            public QueueConfig(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string MaxFlows()
            {
                return _stereotype.GetProperty<string>("Max Flows");
            }

            public string Selector()
            {
                return _stereotype.GetProperty<string>("Selector");
            }

        }

    }
}