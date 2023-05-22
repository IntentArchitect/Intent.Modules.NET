using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Entities.PrivateSetters.MongoDb.Application.Interfaces;
using Entities.PrivateSetters.MongoDb.Domain.Contracts;
using Entities.PrivateSetters.MongoDb.Domain.Entities;
using Entities.PrivateSetters.MongoDb.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace Entities.PrivateSetters.MongoDb.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly ITagRepository _tagRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public InvoiceService(
            IInvoiceRepository invoiceRepository,
            ITagRepository tagRepository,
            IMapper mapper)
        {
            _invoiceRepository = invoiceRepository;
            _tagRepository = tagRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Create(CreateInvoiceDto dto, CancellationToken cancellationToken = default)
        {
            var tags = await _tagRepository.FindAllAsync();

            var invoice = new Invoice(
                date: dto.Date,
                tags: tags
                    .Where(x => dto.TagIds.Contains(x.Id)),
                lines: dto.Lines
                    .Select(x => new LineDataContract(x.Description, x.Quantity)));

            _invoiceRepository.Add(invoice);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<List<InvoiceDto>> GetAll(CancellationToken cancellationToken = default)
        {
            var results = await _invoiceRepository.FindAllAsync();

            return results.MapToInvoiceDtoList(_mapper);
        }

        public void Dispose()
        {
        }
    }
}