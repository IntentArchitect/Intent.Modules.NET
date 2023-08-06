using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Standard.AspNetCore.TestApplication.Application.IntegrationServices.Standard.AspNetCore.TestApplication.Services.Invoices;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.ServiceContract", Version = "2.0")]

namespace Standard.AspNetCore.TestApplication.Application.IntegrationServices
{
    public interface IInvoiceService : IDisposable
    {
        Task<Guid> CreateInvoiceAsync(InvoiceCreateDto dto, CancellationToken cancellationToken = default);
        Task<InvoiceDto?> FindInvoiceByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<InvoiceDto>?> FindInvoicesAsync(CancellationToken cancellationToken = default);
        Task UpdateInvoiceAsync(Guid id, InvoiceUpdateDto dto, CancellationToken cancellationToken = default);
        Task DeleteInvoiceAsync(Guid id, CancellationToken cancellationToken = default);
    }
}