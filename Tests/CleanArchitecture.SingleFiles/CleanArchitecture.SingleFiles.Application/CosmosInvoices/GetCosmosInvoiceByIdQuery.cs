using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.SingleFiles.Application.Common.Interfaces;
using CleanArchitecture.SingleFiles.Domain.Common.Exceptions;
using CleanArchitecture.SingleFiles.Domain.Repositories;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace CleanArchitecture.SingleFiles.Application.CosmosInvoices
{
    public class GetCosmosInvoiceByIdQuery : IRequest<CosmosInvoiceDto>, IQuery
    {
        public GetCosmosInvoiceByIdQuery(string id)
        {
            Id = id;
        }

        public string Id { get; set; }
    }

    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetCosmosInvoiceByIdQueryHandler : IRequestHandler<GetCosmosInvoiceByIdQuery, CosmosInvoiceDto>
    {
        private readonly ICosmosInvoiceRepository _cosmosInvoiceRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetCosmosInvoiceByIdQueryHandler(ICosmosInvoiceRepository cosmosInvoiceRepository, IMapper mapper)
        {
            _cosmosInvoiceRepository = cosmosInvoiceRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<CosmosInvoiceDto> Handle(GetCosmosInvoiceByIdQuery request, CancellationToken cancellationToken)
        {
            var cosmosInvoice = await _cosmosInvoiceRepository.FindByIdAsync(request.Id, cancellationToken);
            if (cosmosInvoice is null)
            {
                throw new NotFoundException($"Could not find CosmosInvoice '{request.Id}'");
            }

            return cosmosInvoice.MapToCosmosInvoiceDto(_mapper);
        }
    }

    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetCosmosInvoiceByIdQueryValidator : AbstractValidator<GetCosmosInvoiceByIdQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetCosmosInvoiceByIdQueryValidator()
        {
            // IntentFully(Match = "ConfigureValidationRules")
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Id)
                .NotNull();
        }
    }
}