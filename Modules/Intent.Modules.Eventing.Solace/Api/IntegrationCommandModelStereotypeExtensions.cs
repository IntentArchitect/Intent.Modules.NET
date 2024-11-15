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
    public static class IntegrationCommandModelStereotypeExtensions
    {
        public static Publishing GetPublishing(this IntegrationCommandModel model)
        {
            var stereotype = model.GetStereotype(Publishing.DefinitionId);
            return stereotype != null ? new Publishing(stereotype) : null;
        }


        public static bool HasPublishing(this IntegrationCommandModel model)
        {
            return model.HasStereotype(Publishing.DefinitionId);
        }

        public static bool TryGetPublishing(this IntegrationCommandModel model, out Publishing stereotype)
        {
            if (!HasPublishing(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Publishing(model.GetStereotype(Publishing.DefinitionId));
            return true;
        }

        public class Publishing
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "56e898f3-74db-486d-86f9-3e885e7509e6";

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