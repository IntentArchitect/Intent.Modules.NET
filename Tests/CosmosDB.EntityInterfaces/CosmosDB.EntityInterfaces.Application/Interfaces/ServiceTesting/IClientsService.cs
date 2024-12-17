using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CosmosDB.EntityInterfaces.Application.Clients;
using CosmosDB.EntityInterfaces.Domain;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace CosmosDB.EntityInterfaces.Application.Interfaces.ServiceTesting
{
    public interface IClientsService
    {
        Task<List<ClientDto>> GetClientsFilteredQuery(ClientType? type, string? name, CancellationToken cancellationToken = default);
    }
}