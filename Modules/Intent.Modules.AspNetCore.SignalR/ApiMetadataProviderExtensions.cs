using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiMetadataProviderExtensions", Version = "1.0")]

namespace Intent.AspNetCore.SignalR.Api
{
    public static class ApiMetadataProviderExtensions
    {
        public static IList<SignalRHubModel> GetSignalRHubModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(SignalRHubModel.SpecializationTypeId)
                .Select(x => new SignalRHubModel(x))
                .ToList();
        }

    }
}