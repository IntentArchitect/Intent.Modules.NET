using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiMetadataPackageExtensions", Version = "1.0")]

namespace Intent.MongoDb.Api
{
    public static class ApiMetadataPackageExtensions
    {
        public static IList<MongoDomainPackageModel> GetMongoDomainPackageModels(this IDesigner designer)
        {
            return designer.GetPackagesOfType(MongoDomainPackageModel.SpecializationTypeId)
                .Select(x => new MongoDomainPackageModel(x))
                .ToList();
        }

        public static bool IsMongoDomainPackageModel(this IPackage package)
        {
            return package?.SpecializationTypeId == MongoDomainPackageModel.SpecializationTypeId;
        }
    }
}