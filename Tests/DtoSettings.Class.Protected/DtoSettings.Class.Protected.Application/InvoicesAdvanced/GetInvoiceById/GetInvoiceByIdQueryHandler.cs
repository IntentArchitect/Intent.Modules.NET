using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DtoSettings.Class.Protected.Domain.Common.Exceptions;
using DtoSettings.Class.Protected.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace DtoSettings.Class.Protected.Application.InvoicesAdvanced.GetInvoiceById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetInvoiceByIdQueryHandler : IRequestHandler<GetInvoiceByIdQuery, InvoiceDto>
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetInvoiceByIdQueryHandler(IInvoiceRepository invoiceRepository, IMapper mapper)
        {
            _invoiceRepository = invoiceRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<InvoiceDto> Handle(GetInvoiceByIdQuery request, CancellationToken cancellationToken)
        {
            var invoice = await _invoiceRepository.FindByIdAsync(request.Id, cancellationToken);
            if (invoice is null)
            {
                throw new NotFoundException($"Could not find Invoice '{request.Id}'");
            }
            return invoice.MapToInvoiceDto(_mapper);
        }
    }
}