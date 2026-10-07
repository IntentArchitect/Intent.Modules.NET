using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Services.Guardrails.Contracts.Api
{
    public static class OperationModelStereotypeExtensions
    {
        public static AsyncConfiguration GetAsyncConfiguration(this OperationModel model)
        {
            var stereotype = model.GetStereotype(AsyncConfiguration.DefinitionId);
            return stereotype != null ? new AsyncConfiguration(stereotype) : null;
        }


        public static bool HasAsyncConfiguration(this OperationModel model)
        {
            return model.HasStereotype(AsyncConfiguration.DefinitionId);
        }

        public static bool TryGetAsyncConfiguration(this OperationModel model, out AsyncConfiguration stereotype)
        {
            if (!HasAsyncConfiguration(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new AsyncConfiguration(model.GetStereotype(AsyncConfiguration.DefinitionId));
            return true;
        }

        public class AsyncConfiguration
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "582f45ce-db44-4e70-bf08-4d69d2b3a760";

            public AsyncConfiguration(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public bool IsAsync()
            {
                return _stereotype.GetProperty<bool>("Is Async");
            }

            public bool IncludeCancellationToken()
            {
                return _stereotype.GetProperty<bool>("Include Cancellation Token");
            }

        }

    }
}