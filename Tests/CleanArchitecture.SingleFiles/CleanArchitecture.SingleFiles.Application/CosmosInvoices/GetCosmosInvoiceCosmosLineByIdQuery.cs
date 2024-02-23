using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.SingleFiles.Application.Common.Interfaces;
using CleanArchitecture.SingleFiles.Domain.Common.Exceptions;
using CleanArchitecture.SingleFiles.Domain.Entities;
using CleanArchitecture.SingleFiles.Domain.Repositories;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace CleanArchitecture.SingleFiles.Application.CosmosInvoices
{
    public class GetCosmosInvoiceCosmosLineByIdQuery : IRequest<CosmosInvoiceCosmosLineDto>, IQuery
    {
        public GetCosmosInvoiceCosmosLineByIdQuery(string cosmosInvoiceId, string id)
        {
            CosmosInvoiceId = cosmosInvoiceId;
            Id = id;
        }

        public string CosmosInvoiceId { get; set; }
        public string Id { get; set; }
    }

    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetCosmosInvoiceCosmosLineByIdQueryHandler : IRequestHandler<GetCosmosInvoiceCosmosLineByIdQuery, CosmosInvoiceCosmosLineDto>
    {
        private readonly ICosmosInvoiceRepository _cosmosInvoiceRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetCosmosInvoiceCosmosLineByIdQueryHandler(ICosmosInvoiceRepository cosmosInvoiceRepository, IMapper mapper)
        {
            _cosmosInvoiceRepository = cosmosInvoiceRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<CosmosInvoiceCosmosLineDto> Handle(
            GetCosmosInvoiceCosmosLineByIdQuery request,
            CancellationToken cancellationToken)
        {
            var aggregateRoot = await _cosmosInvoiceRepository.FindByIdAsync(request.CosmosInvoiceId, cancellationToken);
            if (aggregateRoot is null)
            {
                throw new NotFoundException($"{nameof(CosmosInvoice)} of Id '{request.CosmosInvoiceId}' could not be found");
            }

            var element = aggregateRoot.CosmosLines.FirstOrDefault(p => p.Id == request.Id);
            if (element is null)
            {
                throw new NotFoundException($"Could not find CosmosLine '{request.Id}'");
            }

            return element.MapToCosmosInvoiceCosmosLineDto(_mapper);
        }
    }

    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetCosmosInvoiceCosmosLineByIdQueryValidator : AbstractValidator<GetCosmosInvoiceCosmosLineByIdQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetCosmosInvoiceCosmosLineByIdQueryValidator()
        {
            // IntentFully(Match = "ConfigureValidationRules")
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.CosmosInvoiceId)
                .NotNull();

            RuleFor(v => v.Id)
                .NotNull();
        }
    }
}