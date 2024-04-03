using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Eventing.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Eventing.Kafka.Api
{
    public static class PropertyModelStereotypeExtensions
    {
        public static Key GetKey(this PropertyModel model)
        {
            var stereotype = model.GetStereotype("ea495988-41a8-41f2-944e-24632a06fa70");
            return stereotype != null ? new Key(stereotype) : null;
        }


        public static bool HasKey(this PropertyModel model)
        {
            return model.HasStereotype("ea495988-41a8-41f2-944e-24632a06fa70");
        }

        public static bool TryGetKey(this PropertyModel model, out Key stereotype)
        {
            if (!HasKey(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Key(model.GetStereotype("ea495988-41a8-41f2-944e-24632a06fa70"));
            return true;
        }

        public class Key
        {
            private IStereotype _stereotype;

            public Key(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

        }

    }
}