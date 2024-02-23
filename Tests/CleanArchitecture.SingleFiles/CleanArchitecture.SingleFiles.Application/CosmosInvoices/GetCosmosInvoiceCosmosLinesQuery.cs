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

namespace CleanArchitecture.SingleFiles.Application.CosmosInvoices
{
    public class GetCosmosInvoiceCosmosLinesQuery : IRequest<List<CosmosInvoiceCosmosLineDto>>, IQuery
    {
        public GetCosmosInvoiceCosmosLinesQuery(string cosmosInvoiceId)
        {
            CosmosInvoiceId = cosmosInvoiceId;
        }

        public string CosmosInvoiceId { get; set; }
    }

    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetCosmosInvoiceCosmosLinesQueryHandler : IRequestHandler<GetCosmosInvoiceCosmosLinesQuery, List<CosmosInvoiceCosmosLineDto>>
    {
        private readonly ICosmosInvoiceRepository _cosmosInvoiceRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetCosmosInvoiceCosmosLinesQueryHandler(ICosmosInvoiceRepository cosmosInvoiceRepository, IMapper mapper)
        {
            _cosmosInvoiceRepository = cosmosInvoiceRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<CosmosInvoiceCosmosLineDto>> Handle(
            GetCosmosInvoiceCosmosLinesQuery request,
            CancellationToken cancellationToken)
        {
            var aggregateRoot = await _cosmosInvoiceRepository.FindByIdAsync(request.CosmosInvoiceId, cancellationToken);
            if (aggregateRoot == null)
            {
                throw new InvalidOperationException($"{nameof(CosmosInvoice)} of Id '{request.CosmosInvoiceId}' could not be found");
            }
            return aggregateRoot.CosmosLines.MapToCosmosInvoiceCosmosLineDtoList(_mapper);
        }
    }

    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetCosmosInvoiceCosmosLinesQueryValidator : AbstractValidator<GetCosmosInvoiceCosmosLinesQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetCosmosInvoiceCosmosLinesQueryValidator()
        {
            // IntentFully(Match = "ConfigureValidationRules")
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.CosmosInvoiceId)
                .NotNull();
        }
    }
}