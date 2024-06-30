using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Eventing.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Eventing.Solace.Api
{
    public static class MessageModelStereotypeExtensions
    {
        public static Publishing GetPublishing(this MessageModel model)
        {
            var stereotype = model.GetStereotype("56e898f3-74db-486d-86f9-3e885e7509e6");
            return stereotype != null ? new Publishing(stereotype) : null;
        }


        public static bool HasPublishing(this MessageModel model)
        {
            return model.HasStereotype("56e898f3-74db-486d-86f9-3e885e7509e6");
        }

        public static bool TryGetPublishing(this MessageModel model, out Publishing stereotype)
        {
            if (!HasPublishing(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Publishing(model.GetStereotype("56e898f3-74db-486d-86f9-3e885e7509e6"));
            return true;
        }

        public class Publishing
        {
            private IStereotype _stereotype;

            public Publishing(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string Destination()
            {
                return _stereotype.GetProperty<string>("Destination");
            }

            public int? Priority()
            {
                return _stereotype.GetProperty<int?>("Priority");
            }

        }

    }
}