using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.ApiGateway.Api;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.ApiGateway.Ocelot.Api
{
    public static class ApiGatewayRouteModelStereotypeExtensions
    {
        public static RateLimit GetRateLimit(this ApiGatewayRouteModel model)
        {
            var stereotype = model.GetStereotype(RateLimit.DefinitionId);
            return stereotype != null ? new RateLimit(stereotype) : null;
        }


        public static bool HasRateLimit(this ApiGatewayRouteModel model)
        {
            return model.HasStereotype(RateLimit.DefinitionId);
        }

        public static bool TryGetRateLimit(this ApiGatewayRouteModel model, out RateLimit stereotype)
        {
            if (!HasRateLimit(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new RateLimit(model.GetStereotype(RateLimit.DefinitionId));
            return true;
        }

        public class RateLimit
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "fee00939-5360-4a07-8d80-300a7d9ee81e";

            public RateLimit(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

        }

    }
}