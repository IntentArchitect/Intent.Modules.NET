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
    public class DeleteCosmosInvoiceCosmosLineCommand : IRequest, ICommand
    {
        public DeleteCosmosInvoiceCosmosLineCommand(string cosmosInvoiceId, string id)
        {
            CosmosInvoiceId = cosmosInvoiceId;
            Id = id;
        }

        public string CosmosInvoiceId { get; set; }
        public string Id { get; set; }
    }

    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteCosmosInvoiceCosmosLineCommandHandler : IRequestHandler<DeleteCosmosInvoiceCosmosLineCommand>
    {
        private readonly ICosmosInvoiceRepository _cosmosInvoiceRepository;

        [IntentManaged(Mode.Merge)]
        public DeleteCosmosInvoiceCosmosLineCommandHandler(ICosmosInvoiceRepository cosmosInvoiceRepository)
        {
            _cosmosInvoiceRepository = cosmosInvoiceRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(DeleteCosmosInvoiceCosmosLineCommand request, CancellationToken cancellationToken)
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

            aggregateRoot.CosmosLines.Remove(existingCosmosLine);

            _cosmosInvoiceRepository.Update(aggregateRoot);
        }
    }

    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeleteCosmosInvoiceCosmosLineCommandValidator : AbstractValidator<DeleteCosmosInvoiceCosmosLineCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteCosmosInvoiceCosmosLineCommandValidator()
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