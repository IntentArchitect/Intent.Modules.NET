using CleanArchitecture.Dapr.InvocationClient.Application.IntegrationServices.Contracts.Services.AdvancedMappingSystem.Clients;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.ServiceContract", Version = "2.0")]

namespace CleanArchitecture.Dapr.InvocationClient.Application.IntegrationServices
{
    public interface IClientsService : IDisposable
    {
        Task<string> CreateClientAsync(CreateClientCommand command, CancellationToken cancellationToken = default);
        Task DeleteClientAsync(string id, CancellationToken cancellationToken = default);
        Task<ClientDto> GetClientByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<List<ClientDto>> GetClientsAsync(CancellationToken cancellationToken = default);
        Task UpdateClientAsync(UpdateClientCommand command, CancellationToken cancellationToken = default);
    }
}