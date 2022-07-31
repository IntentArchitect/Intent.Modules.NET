using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Application.MediatR.Api
{
    public static class CommandModelStereotypeExtensions
    {
        public static Authorize GetAuthorize(this CommandModel model)
        {
            var stereotype = model.GetStereotype("Authorize");
            return stereotype != null ? new Authorize(stereotype) : null;
        }


        public static bool HasAuthorize(this CommandModel model)
        {
            return model.HasStereotype("Authorize");
        }

        public static bool TryGetAuthorize(this CommandModel model, out Authorize stereotype)
        {
            if (!HasAuthorize(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Authorize(model.GetStereotype("Authorize"));
            return true;
        }


        public class Authorize
        {
            private IStereotype _stereotype;

            public Authorize(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string Roles()
            {
                return _stereotype.GetProperty<string>("Roles");
            }

            public string Policy()
            {
                return _stereotype.GetProperty<string>("Policy");
            }

        }

    }
}