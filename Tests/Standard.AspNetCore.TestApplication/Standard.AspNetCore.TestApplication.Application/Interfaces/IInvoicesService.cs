using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Standard.AspNetCore.TestApplication.Application.Invoices;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace Standard.AspNetCore.TestApplication.Application.Interfaces
{
    public interface IInvoicesService
    {
        Task<Guid> CreateInvoice(InvoiceCreateDto dto, CancellationToken cancellationToken = default);
        Task<InvoiceDto> FindInvoiceById(Guid id, CancellationToken cancellationToken = default);
        Task<List<InvoiceDto>> FindInvoices(CancellationToken cancellationToken = default);
        Task UpdateInvoice(Guid id, InvoiceUpdateDto dto, CancellationToken cancellationToken = default);
        Task DeleteInvoice(Guid id, CancellationToken cancellationToken = default);
    }
}