using System;
using System.Threading;
using System.Threading.Tasks;
using HttpClientLibrary.Shared.Contracts.CleanArchitecture.SingleFiles.Services.AdvancedMappingCosmosInvoices;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.ServiceContract", Version = "2.0")]

namespace HttpClientLibrary.Shared.Contracts.TestClients
{
    public interface IAdvancedMappingCosmosInvoicesService : IDisposable
    {
        Task<string> CreateCosmosInvoiceAsync(CreateCosmosInvoiceCommand command, CancellationToken cancellationToken = default);
    }
}