using System;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.AspNetCore.Controllers.Api
{
    public static class ServiceModelExtensions
    {
        public static Secured GetSecured(this ServiceModel model)
        {
            var stereotype = model.GetStereotype("Secured");
            return stereotype != null ? new Secured(stereotype) : null;
        }

        public static bool HasSecured(this ServiceModel model)
        {
            return model.HasStereotype("Secured");
        }


        public class Secured
        {
            private IStereotype _stereotype;

            public Secured(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string Roles()
            {
                return _stereotype.GetProperty<string>("Roles");
            }

        }

    }
}