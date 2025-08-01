using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.AspNetCore.OutputCaching.Redis.Api
{
    public static class OperationModelStereotypeExtensions
    {
        public static Caching GetCaching(this OperationModel model)
        {
            var stereotype = model.GetStereotype(Caching.DefinitionId);
            return stereotype != null ? new Caching(stereotype) : null;
        }


        public static bool HasCaching(this OperationModel model)
        {
            return model.HasStereotype(Caching.DefinitionId);
        }

        public static bool TryGetCaching(this OperationModel model, out Caching stereotype)
        {
            if (!HasCaching(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Caching(model.GetStereotype(Caching.DefinitionId));
            return true;
        }

        public class Caching
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "c090804e-8e1c-4121-8209-488982ce6a22";

            public Caching(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string Tags()
            {
                return _stereotype.GetProperty<string>("Tags");
            }

            public IElement Policy()
            {
                return _stereotype.GetProperty<IElement>("Policy");
            }

            public bool Override()
            {
                return _stereotype.GetProperty<bool>("Override");
            }

            public int? Duration()
            {
                return _stereotype.GetProperty<int?>("Duration");
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