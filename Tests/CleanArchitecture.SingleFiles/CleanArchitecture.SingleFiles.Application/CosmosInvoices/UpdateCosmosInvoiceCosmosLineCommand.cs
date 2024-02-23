using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.SingleFiles.Application.Common.Interfaces;
using CleanArchitecture.SingleFiles.Domain.Common.Exceptions;
using CleanArchitecture.SingleFiles.Domain.Entities;
using CleanArchitecture.SingleFiles.Domain.Repositories;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.SingleFiles.Application.CosmosInvoices
{
    public class UpdateCosmosInvoiceCosmosLineCommand : IRequest, ICommand
    {
        public UpdateCosmosInvoiceCosmosLineCommand(string cosmosInvoiceId, string id, string name)
        {
            CosmosInvoiceId = cosmosInvoiceId;
            Id = id;
            Name = name;
        }

        public string CosmosInvoiceId { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
    }

    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateCosmosInvoiceCosmosLineCommandHandler : IRequestHandler<UpdateCosmosInvoiceCosmosLineCommand>
    {
        private readonly ICosmosInvoiceRepository _cosmosInvoiceRepository;

        [IntentManaged(Mode.Merge)]
        public UpdateCosmosInvoiceCosmosLineCommandHandler(ICosmosInvoiceRepository cosmosInvoiceRepository)
        {
            _cosmosInvoiceRepository = cosmosInvoiceRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateCosmosInvoiceCosmosLineCommand request, CancellationToken cancellationToken)
        {
            var aggregateRoot = await _cosmosInvoiceRepository.FindByIdAsync(request.CosmosInvoiceId, cancellationToken);
            if (aggregateRoot is null)
            {
                throw new NotFoundException($"{nameof(CosmosInvoice)} of Id '{request.CosmosInvoiceId}' could not be found");
            }

            var existingCosmosLine = aggregateRoot.CosmosLines.FirstOrDefault(p => p.Id == request.Id);
            if (existingCosmosLine is null)
            {
                throw new NotFoundException($"{nameof(CosmosLine)} of Id '{request.Id}' could not be found associated with {nameof(CosmosInvoice)} of Id '{request.CosmosInvoiceId}'");
            }

#warning No matching field found for CosmosInvoiceId
            existingCosmosLine.Name = request.Name;

            _cosmosInvoiceRepository.Update(aggregateRoot);
        }
    }

    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateCosmosInvoiceCosmosLineCommandValidator : AbstractValidator<UpdateCosmosInvoiceCosmosLineCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateCosmosInvoiceCosmosLineCommandValidator()
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

            RuleFor(v => v.Name)
                .NotNull();
        }
    }
}