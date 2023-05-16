using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Entities.PrivateSetters.EF.CosmosDb.Application.Interfaces;
using Entities.PrivateSetters.EF.CosmosDb.Domain.Contracts;
using Entities.PrivateSetters.EF.CosmosDb.Domain.Entities;
using Entities.PrivateSetters.EF.CosmosDb.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace Entities.PrivateSetters.EF.CosmosDb.Application.Implementation
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
        public async Task Create(CreateInvoiceDto dto)
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
        public async Task<List<InvoiceDto>> GetAll()
        {
            var results = await _invoiceRepository.FindAllAsync();

            foreach (var invoice in results)
            {
                var tags = invoice.Tags.ToList();
            }

            return results.MapToInvoiceDtoList(_mapper);
        }

        public void Dispose()
        {
        }
    }
}