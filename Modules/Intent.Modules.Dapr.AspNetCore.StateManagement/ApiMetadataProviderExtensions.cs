using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiMetadataProviderExtensions", Version = "1.0")]

namespace Intent.Dapr.AspNetCore.StateManagement.Api
{
    public static class ApiMetadataProviderExtensions
    {
        public static IList<StateStoreModel> GetStateStoreModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(StateStoreModel.SpecializationTypeId)
                .Select(x => new StateStoreModel(x))
                .ToList();
        }

    }
}