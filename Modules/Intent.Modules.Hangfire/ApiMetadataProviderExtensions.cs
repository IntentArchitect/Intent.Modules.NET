using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiMetadataProviderExtensions", Version = "1.0")]

namespace Intent.Modules.Hangfire.Api
{
    public static class ApiMetadataProviderExtensions
    {
        public static IList<HangfireConfigurationModel> GetHangfireConfigurationModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(HangfireConfigurationModel.SpecializationTypeId)
                .Select(x => new HangfireConfigurationModel(x))
                .ToList();
        }
        public static IList<HangfireJobModel> GetHangfireJobModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(HangfireJobModel.SpecializationTypeId)
                .Select(x => new HangfireJobModel(x))
                .ToList();
        }

        public static IList<HangfireQueueModel> GetHangfireQueueModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(HangfireQueueModel.SpecializationTypeId)
                .Select(x => new HangfireQueueModel(x))
                .ToList();
        }

    }
}