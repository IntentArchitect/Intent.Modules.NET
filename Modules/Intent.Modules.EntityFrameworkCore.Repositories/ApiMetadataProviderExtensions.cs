using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiMetadataProviderExtensions", Version = "1.0")]

namespace Intent.EntityFrameworkCore.Repositories.Api
{
    public static class ApiMetadataProviderExtensions
    {
        public static IList<StoredProcedureModel> GetStoredProcedureModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(StoredProcedureModel.SpecializationTypeId)
                .Select(x => new StoredProcedureModel(x))
                .ToList();
        }

    }
}