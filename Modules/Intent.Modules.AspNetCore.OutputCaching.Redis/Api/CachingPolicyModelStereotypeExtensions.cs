using System;
using System.Collections.Generic;
using System.Linq;
using Intent.IArchitect.Agent.Persistence;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.AspNetCore.OutputCaching.Redis.Api
{
    public static class CachingPolicyModelStereotypeExtensions
    {
        public static CachingConfig GetCachingConfig(this CachingPolicyModel model)
        {
            var stereotype = model.GetStereotype("56f48881-3e17-43bf-aead-e222a8f725fd");
            return stereotype != null ? new CachingConfig(stereotype) : null;
        }


        public static bool HasCachingConfig(this CachingPolicyModel model)
        {
            return model.HasStereotype("56f48881-3e17-43bf-aead-e222a8f725fd");
        }

        public static bool TryGetCachingConfig(this CachingPolicyModel model, out CachingConfig stereotype)
        {
            if (!HasCachingConfig(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new CachingConfig(model.GetStereotype("56f48881-3e17-43bf-aead-e222a8f725fd"));
            return true;
        }

        public class CachingConfig
        {
            private IStereotype _stereotype;

            public CachingConfig(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public int? Duration()
            {
                return _stereotype.GetProperty<int?>("Duration");
            }

            public string Tags()
            {
                return _stereotype.GetProperty<string>("Tags");
            }

            public string VaryByQueryKeys()
            {
                return _stereotype.GetProperty<string>("Vary By Query Keys");
            }

            public string VaryByHeaderNames()
            {
                return _stereotype.GetProperty<string>("Vary By Header Names");
            }

            public string VaryByRouteValueNames()
            {
                return _stereotype.GetProperty<string>("Vary By Route Value Names");
            }

            public bool NoCaching()
            {
                return _stereotype.GetProperty<bool>("No Caching");
            }
        }

    }
}