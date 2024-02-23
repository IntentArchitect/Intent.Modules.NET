using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.SingleFiles.Application.Common.Interfaces;
using CleanArchitecture.SingleFiles.Domain.Repositories;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace CleanArchitecture.SingleFiles.Application.EfInvoices
{
    public class GetEfInvoicesQuery : IRequest<List<EfInvoiceDto>>, IQuery
    {
        public GetEfInvoicesQuery()
        {
        }
    }

    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetEfInvoicesQueryHandler : IRequestHandler<GetEfInvoicesQuery, List<EfInvoiceDto>>
    {
        private readonly IEfInvoiceRepository _efInvoiceRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetEfInvoicesQueryHandler(IEfInvoiceRepository efInvoiceRepository, IMapper mapper)
        {
            _efInvoiceRepository = efInvoiceRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<EfInvoiceDto>> Handle(GetEfInvoicesQuery request, CancellationToken cancellationToken)
        {
            var efInvoices = await _efInvoiceRepository.FindAllAsync(cancellationToken);
            return efInvoices.MapToEfInvoiceDtoList(_mapper);
        }
    }

    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetEfInvoicesQueryValidator : AbstractValidator<GetEfInvoicesQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetEfInvoicesQueryValidator()
        {
            // IntentFully(Match = "ConfigureValidationRules")
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}