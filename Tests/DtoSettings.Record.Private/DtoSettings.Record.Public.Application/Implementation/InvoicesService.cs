using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DtoSettings.Record.Public.Application.Interfaces;
using DtoSettings.Record.Public.Application.Invoices;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace DtoSettings.Record.Public.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class InvoicesService : IInvoicesService
    {
        [IntentManaged(Mode.Merge)]
        public InvoicesService()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<Guid> CreateInvoice(InvoiceCreateDto dto, CancellationToken cancellationToken = default)
        {
            // TODO: Implement CreateInvoice (InvoicesService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<InvoiceDto> FindInvoiceById(Guid id, CancellationToken cancellationToken = default)
        {
            // TODO: Implement FindInvoiceById (InvoicesService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<List<InvoiceDto>> FindInvoices(CancellationToken cancellationToken = default)
        {
            // TODO: Implement FindInvoices (InvoicesService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task UpdateInvoice(Guid id, InvoiceUpdateDto dto, CancellationToken cancellationToken = default)
        {
            // TODO: Implement UpdateInvoice (InvoicesService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task DeleteInvoice(Guid id, CancellationToken cancellationToken = default)
        {
            // TODO: Implement DeleteInvoice (InvoicesService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }
    }
}