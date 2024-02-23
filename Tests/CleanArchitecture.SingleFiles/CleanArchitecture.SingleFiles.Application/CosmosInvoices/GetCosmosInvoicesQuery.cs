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

namespace CleanArchitecture.SingleFiles.Application.CosmosInvoices
{
    public class GetCosmosInvoicesQuery : IRequest<List<CosmosInvoiceDto>>, IQuery
    {
        public GetCosmosInvoicesQuery()
        {
        }
    }

    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetCosmosInvoicesQueryHandler : IRequestHandler<GetCosmosInvoicesQuery, List<CosmosInvoiceDto>>
    {
        private readonly ICosmosInvoiceRepository _cosmosInvoiceRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetCosmosInvoicesQueryHandler(ICosmosInvoiceRepository cosmosInvoiceRepository, IMapper mapper)
        {
            _cosmosInvoiceRepository = cosmosInvoiceRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<CosmosInvoiceDto>> Handle(
            GetCosmosInvoicesQuery request,
            CancellationToken cancellationToken)
        {
            var cosmosInvoices = await _cosmosInvoiceRepository.FindAllAsync(cancellationToken);
            return cosmosInvoices.MapToCosmosInvoiceDtoList(_mapper);
        }
    }

    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetCosmosInvoicesQueryValidator : AbstractValidator<GetCosmosInvoicesQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetCosmosInvoicesQueryValidator()
        {
            // IntentFully(Match = "ConfigureValidationRules")
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}