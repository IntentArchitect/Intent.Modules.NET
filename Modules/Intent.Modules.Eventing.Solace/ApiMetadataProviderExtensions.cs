using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiMetadataProviderExtensions", Version = "1.0")]

namespace Intent.Eventing.Solace.Api
{
    public static class ApiMetadataProviderExtensions
    {
        public static IList<QueueModel> GetQueueModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(QueueModel.SpecializationTypeId)
                .Select(x => new QueueModel(x))
                .ToList();
        }

    }
}