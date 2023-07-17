using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.ServiceContract", Version = "1.0")]

namespace Standard.AspNetCore.TestApplication.Application.IntegrationServices.InvoiceServiceProxy
{
    public interface IInvoiceServiceProxyClient : IDisposable
    {
        Task<Guid> CreateInvoiceAsync(InvoiceCreateDto dto, CancellationToken cancellationToken = default);
        Task<InvoiceDto?> FindInvoiceByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<InvoiceDto>?> FindInvoicesAsync(CancellationToken cancellationToken = default);
        Task UpdateInvoiceAsync(Guid id, InvoiceUpdateDto dto, CancellationToken cancellationToken = default);
        Task DeleteInvoiceAsync(Guid id, CancellationToken cancellationToken = default);
    }
}