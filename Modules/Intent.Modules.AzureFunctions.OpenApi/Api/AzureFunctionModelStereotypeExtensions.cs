using System;
using System.Collections.Generic;
using System.Linq;
using Intent.AzureFunctions.Api;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.AzureFunctions.OpenApi.Api
{
    public static class AzureFunctionModelStereotypeExtensions
    {
        public static OpenAPIOperation GetOpenAPIOperation(this AzureFunctionModel model)
        {
            var stereotype = model.GetStereotype(OpenAPIOperation.DefinitionId);
            return stereotype != null ? new OpenAPIOperation(stereotype) : null;
        }


        public static bool HasOpenAPIOperation(this AzureFunctionModel model)
        {
            return model.HasStereotype(OpenAPIOperation.DefinitionId);
        }

        public static bool TryGetOpenAPIOperation(this AzureFunctionModel model, out OpenAPIOperation stereotype)
        {
            if (!HasOpenAPIOperation(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new OpenAPIOperation(model.GetStereotype(OpenAPIOperation.DefinitionId));
            return true;
        }

        public class OpenAPIOperation
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "d0dce09b-f93f-4b45-aeaf-106397bee14d";

            public OpenAPIOperation(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string Summary()
            {
                return _stereotype.GetProperty<string>("Summary");
            }

            public string Description()
            {
                return _stereotype.GetProperty<string>("Description");
            }

            public string Tags()
            {
                return _stereotype.GetProperty<string>("Tags");
            }

            public bool Deprecated()
            {
                return _stereotype.GetProperty<bool>("Deprecated");
            }

        }

    }
}