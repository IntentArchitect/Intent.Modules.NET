using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiMetadataProviderExtensions", Version = "1.0")]

namespace Intent.AzureFunctions.Api
{
    public static class ApiMetadataProviderExtensions
    {
        public static IList<AzureFunctionModel> GetAzureFunctionModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(AzureFunctionModel.SpecializationTypeId)
                .Select(x => new AzureFunctionModel(x))
                .ToList();
        }

    }
}