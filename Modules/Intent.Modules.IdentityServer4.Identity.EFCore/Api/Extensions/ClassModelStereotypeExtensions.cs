using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.IdentityServer4.Identity.EFCore.Api
{
    public static class ClassModelStereotypeExtensions
    {
        public static IdentityUser GetIdentityUser(this ClassModel model)
        {
            var stereotype = model.GetStereotype("Identity User");
            return stereotype != null ? new IdentityUser(stereotype) : null;
        }

        public static IReadOnlyCollection<IdentityUser> GetIdentityUsers(this ClassModel model)
        {
            var stereotypes = model
                .GetStereotypes("Identity User")
                .Select(stereotype => new IdentityUser(stereotype))
                .ToArray();

            return stereotypes;
        }


        public static bool HasIdentityUser(this ClassModel model)
        {
            return model.HasStereotype("Identity User");
        }


        public class IdentityUser
        {
            private IStereotype _stereotype;

            public IdentityUser(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

        }

    }
}