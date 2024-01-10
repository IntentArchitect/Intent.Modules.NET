using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DtoSettings.Class.Init.Application.Interfaces;
using DtoSettings.Class.Init.Application.Invoices;
using DtoSettings.Class.Init.Domain.Common;
using DtoSettings.Class.Init.Domain.Common.Exceptions;
using DtoSettings.Class.Init.Domain.Entities;
using DtoSettings.Class.Init.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace DtoSettings.Class.Init.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class InvoicesService : IInvoicesService
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
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
                InvoiceLines = dto.InvoiceLines.Select(CreateInvoiceLine).ToList(),
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
            existingInvoice.InvoiceLines = UpdateHelper.CreateOrUpdateCollection(existingInvoice.InvoiceLines, dto.InvoiceLines, (e, d) => e.Id == d.Id, CreateOrUpdateInvoiceLine);
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

        public void Dispose()
        {
        }

        [IntentManaged(Mode.Fully)]
        private InvoiceLine CreateInvoiceLine(InvoiceLineCreateDto dto)
        {
            return new InvoiceLine
            {
                Description = dto.Description,
                Amount = dto.Amount,
                Currency = dto.Currency,
            };
        }

        [IntentManaged(Mode.Fully)]
        private static InvoiceLine CreateOrUpdateInvoiceLine(InvoiceLine? entity, InvoiceLineDto dto)
        {
            if (dto == null)
            {
                return null;
            }

            entity ??= new InvoiceLine();
            entity.Description = dto.Description;
            entity.Amount = dto.Amount;
            entity.Currency = dto.Currency;
            entity.InvoiceId = dto.InvoiceId;

            return entity;
        }
    }
}