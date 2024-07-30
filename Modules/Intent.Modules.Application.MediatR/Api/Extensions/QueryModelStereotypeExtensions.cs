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
    public static class QueryModelStereotypeExtensions
    {
        public static Authorize GetAuthorize(this QueryModel model)
        {
            var stereotype = model.GetStereotype("b06358cd-aed3-4c39-96cf-abb131e4ecde");
            return stereotype != null ? new Authorize(stereotype) : null;
        }


        public static bool HasAuthorize(this QueryModel model)
        {
            return model.HasStereotype("b06358cd-aed3-4c39-96cf-abb131e4ecde");
        }

        public static bool TryGetAuthorize(this QueryModel model, out Authorize stereotype)
        {
            if (!HasAuthorize(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Authorize(model.GetStereotype("b06358cd-aed3-4c39-96cf-abb131e4ecde"));
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

            public IElement[] SecurityRoles()
            {
                return _stereotype.GetProperty<IElement[]>("Security Roles") ?? new IElement[0];
            }

            public IElement[] SecurityPolicies()
            {
                return _stereotype.GetProperty<IElement[]>("Security Policies") ?? new IElement[0];
            }

        }

    }
}