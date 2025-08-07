using Intent.RoslynWeaver.Attributes;
using MudBlazor.ExampleApp.Client.HttpClients.Contracts.Services.Collections;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.ServiceContract", Version = "2.0")]

namespace MudBlazor.ExampleApp.Client.HttpClients
{
    public interface ICollectionsService : IDisposable
    {
        Task<List<ResponseDto>> GetDataWithCollectionParamsAsync(List<int> intCollection, List<string> stringCollection, int intValue, CancellationToken cancellationToken = default);
        Task<List<ResponseDto>> GetDataSingleCollectionAsync(List<int> intCollection, string stringValue, CancellationToken cancellationToken = default);
    }
}