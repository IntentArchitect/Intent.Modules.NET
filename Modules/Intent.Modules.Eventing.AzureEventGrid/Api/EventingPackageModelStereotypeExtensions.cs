using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Eventing.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Eventing.AzureEventGrid.Api
{
    public static class EventingPackageModelStereotypeExtensions
    {
        public static EventDomain GetEventDomain(this EventingPackageModel model)
        {
            var stereotype = model.GetStereotype(EventDomain.DefinitionId);
            return stereotype != null ? new EventDomain(stereotype) : null;
        }


        public static bool HasEventDomain(this EventingPackageModel model)
        {
            return model.HasStereotype(EventDomain.DefinitionId);
        }

        public static bool TryGetEventDomain(this EventingPackageModel model, out EventDomain stereotype)
        {
            if (!HasEventDomain(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new EventDomain(model.GetStereotype(EventDomain.DefinitionId));
            return true;
        }

        public class EventDomain
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "b440c77b-3bde-4a96-bcb6-3289a23e5b1d";

            public EventDomain(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string DomainName()
            {
                return _stereotype.GetProperty<string>("Domain Name");
            }

        }

    }
}