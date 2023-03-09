using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.CosmosDb.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiMetadataProviderExtensions", Version = "1.0")]

namespace Intent.Modules.CosmosDb
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