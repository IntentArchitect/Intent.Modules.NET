using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.Dapr.Domain.Common.Exceptions;
using CleanArchitecture.Dapr.Domain.Entities;
using CleanArchitecture.Dapr.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CleanArchitecture.Dapr.Application.OldMappingSystem.Invoices.GetInvoiceInvoiceLineById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetInvoiceInvoiceLineByIdQueryHandler : IRequestHandler<GetInvoiceInvoiceLineByIdQuery, InvoiceInvoiceLineDto>
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetInvoiceInvoiceLineByIdQueryHandler(IInvoiceRepository invoiceRepository, IMapper mapper)
        {
            _invoiceRepository = invoiceRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<InvoiceInvoiceLineDto> Handle(
            GetInvoiceInvoiceLineByIdQuery request,
            CancellationToken cancellationToken)
        {
            var aggregateRoot = await _invoiceRepository.FindByIdAsync(request.InvoiceId, cancellationToken);
            if (aggregateRoot is null)
            {
                throw new NotFoundException($"{nameof(Invoice)} of Id '{request.InvoiceId}' could not be found");
            }

            var element = aggregateRoot.InvoiceLines.FirstOrDefault(p => p.Id == request.Id);
            if (element is null)
            {
                throw new NotFoundException($"Could not find InvoiceLine '{request.Id}'");
            }

            return element.MapToInvoiceInvoiceLineDto(_mapper);
        }
    }
}