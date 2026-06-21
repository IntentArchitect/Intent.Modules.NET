using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Eventing.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Eventing.NServiceBus.Api
{
    public static class IntegrationCommandModelStereotypeExtensions
    {
        public static NServiceBus GetNServiceBus(this IntegrationCommandModel model)
        {
            var stereotype = model.GetStereotype(NServiceBus.DefinitionId);
            return stereotype != null ? new NServiceBus(stereotype) : null;
        }


        public static bool HasNServiceBus(this IntegrationCommandModel model)
        {
            return model.HasStereotype(NServiceBus.DefinitionId);
        }

        public static bool TryGetNServiceBus(this IntegrationCommandModel model, out NServiceBus stereotype)
        {
            if (!HasNServiceBus(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new NServiceBus(model.GetStereotype(NServiceBus.DefinitionId));
            return true;
        }

        public class NServiceBus
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "c45c1bba-04ce-4ba3-88e6-c69b6ce3d1ca";

            public NServiceBus(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string EndpointName()
            {
                return _stereotype.GetProperty<string>("Endpoint Name");
            }

        }

    }
}