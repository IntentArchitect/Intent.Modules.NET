using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiMetadataProviderExtensions", Version = "1.0")]

namespace Intent.AspNetCore.OutputCaching.Redis.Api
{
    public static class ApiMetadataProviderExtensions
    {
        public static IList<CachingPoliciesModel> GetCachingPoliciesModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(CachingPoliciesModel.SpecializationTypeId)
                .Select(x => new CachingPoliciesModel(x))
                .ToList();
        }
        public static IList<CachingPolicyModel> GetCachingPolicyModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(CachingPolicyModel.SpecializationTypeId)
                .Select(x => new CachingPolicyModel(x))
                .ToList();
        }

    }
}