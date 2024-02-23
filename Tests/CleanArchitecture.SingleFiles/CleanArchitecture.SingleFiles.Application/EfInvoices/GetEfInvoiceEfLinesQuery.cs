using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.SingleFiles.Application.Common.Interfaces;
using CleanArchitecture.SingleFiles.Domain.Entities;
using CleanArchitecture.SingleFiles.Domain.Repositories;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace CleanArchitecture.SingleFiles.Application.EfInvoices
{
    public class GetEfInvoiceEfLinesQuery : IRequest<List<EfInvoiceEfLineDto>>, IQuery
    {
        public GetEfInvoiceEfLinesQuery(Guid efInvoicesId)
        {
            EfInvoicesId = efInvoicesId;
        }

        public Guid EfInvoicesId { get; set; }
    }

    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetEfInvoiceEfLinesQueryHandler : IRequestHandler<GetEfInvoiceEfLinesQuery, List<EfInvoiceEfLineDto>>
    {
        private readonly IEfInvoiceRepository _efInvoiceRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetEfInvoiceEfLinesQueryHandler(IEfInvoiceRepository efInvoiceRepository, IMapper mapper)
        {
            _efInvoiceRepository = efInvoiceRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<EfInvoiceEfLineDto>> Handle(
            GetEfInvoiceEfLinesQuery request,
            CancellationToken cancellationToken)
        {
            var aggregateRoot = await _efInvoiceRepository.FindByIdAsync(request.EfInvoicesId, cancellationToken);
            if (aggregateRoot == null)
            {
                throw new InvalidOperationException($"{nameof(EfInvoice)} of Id '{request.EfInvoicesId}' could not be found");
            }
            return aggregateRoot.EfLines.MapToEfInvoiceEfLineDtoList(_mapper);
        }
    }

    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetEfInvoiceEfLinesQueryValidator : AbstractValidator<GetEfInvoiceEfLinesQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetEfInvoiceEfLinesQueryValidator()
        {
            // IntentFully(Match = "ConfigureValidationRules")
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}