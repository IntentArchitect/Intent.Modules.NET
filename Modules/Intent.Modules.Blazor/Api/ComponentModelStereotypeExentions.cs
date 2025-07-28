using Intent.Metadata.Models;
using Intent.Modelers.UI.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Blazor.Api
{
    internal static class ComponentModelStereotypeExtensions
    {
        public static Secured GetSecured(this ComponentModel model)
        {
            var stereotype = model.GetStereotype(Secured.DefinitionId);
            return stereotype != null ? new Secured(stereotype) : null;
        }

        public static IReadOnlyCollection<Secured> GetSecureds(this ComponentModel model)
        {
            var stereotypes = model
                .GetStereotypes(Secured.DefinitionId)
                .Select(stereotype => new Secured(stereotype))
                .ToArray();

            return stereotypes;
        }


        public static bool HasSecured(this ComponentModel model)
        {
            return model.HasStereotype(Secured.DefinitionId);
        }

        public static bool TryGetSecured(this ComponentModel model, out Secured stereotype)
        {
            if (!HasSecured(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Secured(model.GetStereotype(Secured.DefinitionId));
            return true;
        }

        public static Unsecured GetUnsecured(this ComponentModel model)
        {
            var stereotype = model.GetStereotype(Unsecured.DefinitionId);
            return stereotype != null ? new Unsecured(stereotype) : null;
        }


        public static bool HasUnsecured(this ComponentModel model)
        {
            return model.HasStereotype(Unsecured.DefinitionId);
        }

        public static bool TryGetUnsecured(this ComponentModel model, out Unsecured stereotype)
        {
            if (!HasUnsecured(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Unsecured(model.GetStereotype(Unsecured.DefinitionId));
            return true;
        }

        public class Secured
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "a9eade71-1d56-4be7-a80c-81046c0c978b";

            public Secured(IStereotype stereotype)
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

            public IElement SecurityPolicies()
            {
                return _stereotype.GetProperty<IElement>("Security Policies");
            }

        }

        public class Unsecured
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "8b65c29e-1448-43ac-a92a-0e0f86efd6c6";

            public Unsecured(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

        }

    }

}
