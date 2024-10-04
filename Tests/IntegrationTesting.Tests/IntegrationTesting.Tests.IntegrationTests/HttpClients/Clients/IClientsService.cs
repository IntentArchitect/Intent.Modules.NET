using IntegrationTesting.Tests.IntegrationTests.Services.Clients;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ProxyServiceContract", Version = "1.0")]

namespace IntegrationTesting.Tests.IntegrationTests.HttpClients.Clients
{
    public interface IClientsService : IDisposable
    {
        Task<Guid> CreateClientAsync(CreateClientCommand command, CancellationToken cancellationToken = default);
        Task DeleteClientAsync(Guid id, CancellationToken cancellationToken = default);
        Task UpdateClientAsync(Guid id, UpdateClientCommand command, CancellationToken cancellationToken = default);
        Task<ClientDto> GetClientByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<ClientDto>> GetClientsAsync(CancellationToken cancellationToken = default);
    }
}