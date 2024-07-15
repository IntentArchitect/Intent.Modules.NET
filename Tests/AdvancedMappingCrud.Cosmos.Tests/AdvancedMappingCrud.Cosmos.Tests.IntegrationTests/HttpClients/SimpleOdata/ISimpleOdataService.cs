using AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.Services.SimpleOdata;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ProxyServiceContract", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.HttpClients.SimpleOdata
{
    public interface ISimpleOdataService : IDisposable
    {
        Task<string> CreateSimpleOdataAsync(CreateSimpleOdataCommand command, CancellationToken cancellationToken = default);
        Task DeleteSimpleOdataAsync(string id, CancellationToken cancellationToken = default);
        Task UpdateSimpleOdataAsync(string id, UpdateSimpleOdataCommand command, CancellationToken cancellationToken = default);
        Task<SimpleOdataDto> GetSimpleOdataByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<List<SimpleOdataDto>> GetSimpleOdataAsync(CancellationToken cancellationToken = default);
    }
}