using Intent.RoslynWeaver.Attributes;
using MudBlazor.ExampleApp.Client.HttpClients.Common;
using MudBlazor.ExampleApp.Client.HttpClients.Contracts.Services.Invoices;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.ServiceContract", Version = "2.0")]

namespace MudBlazor.ExampleApp.Client.HttpClients
{
    public interface IInvoiceService : IDisposable
    {
        Task<Guid> CreateInvoiceAsync(CreateInvoiceCommand command, CancellationToken cancellationToken = default);
        Task DeleteInvoiceAsync(Guid id, CancellationToken cancellationToken = default);
        Task UpdateInvoiceAsync(Guid id, UpdateInvoiceCommand command, CancellationToken cancellationToken = default);
        Task<PagedResult<InvoiceDto>> GetInvoicesAsync(int pageNo, int pageSize, string? orderBy, CancellationToken cancellationToken = default);
        Task<InvoiceDto> GetInvoiceByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task CreateInvoiceConvolutedAsync(CreateInvoiceConvolutedCommand command, CancellationToken cancellationToken = default);
    }
}