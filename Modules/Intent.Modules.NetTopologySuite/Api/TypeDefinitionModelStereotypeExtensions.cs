using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.NetTopologySuite.Api
{
    public static class TypeDefinitionModelStereotypeExtensions
    {
        public static Geometry GetGeometry(this TypeDefinitionModel model)
        {
            var stereotype = model.GetStereotype("7fdf11a8-5962-472c-8916-ebb8323de8b7");
            return stereotype != null ? new Geometry(stereotype) : null;
        }


        public static bool HasGeometry(this TypeDefinitionModel model)
        {
            return model.HasStereotype("7fdf11a8-5962-472c-8916-ebb8323de8b7");
        }

        public static bool TryGetGeometry(this TypeDefinitionModel model, out Geometry stereotype)
        {
            if (!HasGeometry(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Geometry(model.GetStereotype("7fdf11a8-5962-472c-8916-ebb8323de8b7"));
            return true;
        }

        public class Geometry
        {
            private IStereotype _stereotype;

            public Geometry(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

        }

    }
}