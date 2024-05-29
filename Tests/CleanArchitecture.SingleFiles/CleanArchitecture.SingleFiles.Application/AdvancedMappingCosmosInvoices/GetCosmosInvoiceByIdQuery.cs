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

namespace CleanArchitecture.SingleFiles.Application.AdvancedMappingCosmosInvoices
{
    public class GetCosmosInvoiceByIdQuery : IRequest<CosmosInvoiceDto>, IQuery
    {
        private readonly ICosmosInvoiceRepository _cosmosInvoiceRepository;
        private readonly IMapper _mapper;

        public GetCosmosInvoiceByIdQuery(string id, ICosmosInvoiceRepository cosmosInvoiceRepository, IMapper mapper)
        {
            Id = id;
            _cosmosInvoiceRepository = cosmosInvoiceRepository;
            _mapper = mapper;
        }

        public string Id { get; set; }
    }

    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetCosmosInvoiceByIdQueryHandler : IRequestHandler<GetCosmosInvoiceByIdQuery, CosmosInvoiceDto>
    {
        [IntentManaged(Mode.Merge)]
        public GetCosmosInvoiceByIdQueryHandler()
        {
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
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Id)
                .NotNull();
        }
    }
}