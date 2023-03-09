using System.Collections.Generic;
using System.Linq;
using Intent.CosmosDb.Api;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiMetadataProviderExtensions", Version = "1.0")]

namespace Intent.CosmosDb.Api
{
    public static class ApiMetadataProviderExtensions
    {
        public static IList<CosmosDBDiagramModel> GetCosmosDBDiagramModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(CosmosDBDiagramModel.SpecializationTypeId)
                .Select(x => new CosmosDBDiagramModel(x))
                .ToList();
        }

    }
}