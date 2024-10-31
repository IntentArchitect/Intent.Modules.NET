using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using Standard.AspNetCore.TestApplication.Application.Interfaces;
using Standard.AspNetCore.TestApplication.Application.Invoices;
using Standard.AspNetCore.TestApplication.Domain.Common.Exceptions;
using Standard.AspNetCore.TestApplication.Domain.Entities;
using Standard.AspNetCore.TestApplication.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace Standard.AspNetCore.TestApplication.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class InvoicesService : IInvoicesService
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public InvoicesService(IInvoiceRepository invoiceRepository, IMapper mapper)
        {
            _invoiceRepository = invoiceRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> CreateInvoice(InvoiceCreateDto dto, CancellationToken cancellationToken = default)
        {
            var newInvoice = new Invoice
            {
                Number = dto.Number,
            };
            _invoiceRepository.Add(newInvoice);
            await _invoiceRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return newInvoice.Id;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<InvoiceDto> FindInvoiceById(Guid id, CancellationToken cancellationToken = default)
        {
            var element = await _invoiceRepository.FindByIdAsync(id, cancellationToken);

            if (element is null)
            {
                throw new NotFoundException($"Could not find Invoice {id}");
            }
            return element.MapToInvoiceDto(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<InvoiceDto>> FindInvoices(CancellationToken cancellationToken = default)
        {
            var elements = await _invoiceRepository.FindAllAsync(cancellationToken);
            return elements.MapToInvoiceDtoList(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task UpdateInvoice(Guid id, InvoiceUpdateDto dto, CancellationToken cancellationToken = default)
        {
            var existingInvoice = await _invoiceRepository.FindByIdAsync(id, cancellationToken);

            if (existingInvoice is null)
            {
                throw new NotFoundException($"Could not find Invoice {id}");
            }
            existingInvoice.Number = dto.Number;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task DeleteInvoice(Guid id, CancellationToken cancellationToken = default)
        {
            var existingInvoice = await _invoiceRepository.FindByIdAsync(id, cancellationToken);

            if (existingInvoice is null)
            {
                throw new NotFoundException($"Could not find Invoice {id}");
            }
            _invoiceRepository.Remove(existingInvoice);
        }
    }
}