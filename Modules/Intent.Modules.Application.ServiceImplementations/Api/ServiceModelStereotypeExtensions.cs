using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Application.ServiceImplementations.Api
{
    public static class ServiceModelStereotypeExtensions
    {
        public static ServiceSettings GetServiceSettings(this ServiceModel model)
        {
            var stereotype = model.GetStereotype("Service Settings");
            return stereotype != null ? new ServiceSettings(stereotype) : null;
        }


        public static bool HasServiceSettings(this ServiceModel model)
        {
            return model.HasStereotype("Service Settings");
        }

        public static bool TryGetServiceSettings(this ServiceModel model, out ServiceSettings stereotype)
        {
            if (!HasServiceSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new ServiceSettings(model.GetStereotype("Service Settings"));
            return true;
        }

        public class ServiceSettings
        {
            private IStereotype _stereotype;

            public ServiceSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public bool ContractOnly()
            {
                return _stereotype.GetProperty<bool>("Contract Only");
            }

        }

    }
}