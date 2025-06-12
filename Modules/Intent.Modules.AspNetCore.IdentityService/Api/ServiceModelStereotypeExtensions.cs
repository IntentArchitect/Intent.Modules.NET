using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.AspNetCore.IdentityService.Api
{
    public static class ServiceModelStereotypeExtensions
    {
        public static IdentityServiceHandler GetIdentityServiceHandler(this ServiceModel model)
        {
            var stereotype = model.GetStereotype(IdentityServiceHandler.DefinitionId);
            return stereotype != null ? new IdentityServiceHandler(stereotype) : null;
        }


        public static bool HasIdentityServiceHandler(this ServiceModel model)
        {
            return model.HasStereotype(IdentityServiceHandler.DefinitionId);
        }

        public static bool TryGetIdentityServiceHandler(this ServiceModel model, out IdentityServiceHandler stereotype)
        {
            if (!HasIdentityServiceHandler(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new IdentityServiceHandler(model.GetStereotype(IdentityServiceHandler.DefinitionId));
            return true;
        }

        public class IdentityServiceHandler
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "8ef84001-167a-4cbb-8950-e519937e7981";

            public IdentityServiceHandler(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

        }

    }
}