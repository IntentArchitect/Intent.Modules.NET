using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiMetadataProviderExtensions", Version = "1.0")]

namespace Intent.Eventing.Contracts.Api
{
    public static class ApiMetadataProviderExtensions
    {
        public static IList<MessageBusProviderModel> GetMessageBusProviderModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(MessageBusProviderModel.SpecializationTypeId)
                .Select(x => new MessageBusProviderModel(x))
                .ToList();
        }

    }
}