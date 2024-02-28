using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiMetadataProviderExtensions", Version = "1.0")]

namespace Intent.QuartzScheduler.Api
{
    public static class ApiMetadataProviderExtensions
    {
        public static IList<ScheduledJobModel> GetScheduledJobModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(ScheduledJobModel.SpecializationTypeId)
                .Select(x => new ScheduledJobModel(x))
                .ToList();
        }

    }
}