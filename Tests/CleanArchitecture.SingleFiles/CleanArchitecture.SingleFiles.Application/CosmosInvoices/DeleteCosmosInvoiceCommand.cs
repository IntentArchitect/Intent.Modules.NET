using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.SingleFiles.Application.Common.Interfaces;
using CleanArchitecture.SingleFiles.Domain.Common.Exceptions;
using CleanArchitecture.SingleFiles.Domain.Repositories;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.SingleFiles.Application.CosmosInvoices
{
    public class DeleteCosmosInvoiceCommand : IRequest, ICommand
    {
        public DeleteCosmosInvoiceCommand(string id)
        {
            Id = id;
        }

        public string Id { get; set; }
    }

    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteCosmosInvoiceCommandHandler : IRequestHandler<DeleteCosmosInvoiceCommand>
    {
        private readonly ICosmosInvoiceRepository _cosmosInvoiceRepository;

        [IntentManaged(Mode.Merge)]
        public DeleteCosmosInvoiceCommandHandler(ICosmosInvoiceRepository cosmosInvoiceRepository)
        {
            _cosmosInvoiceRepository = cosmosInvoiceRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(DeleteCosmosInvoiceCommand request, CancellationToken cancellationToken)
        {
            var existingCosmosInvoice = await _cosmosInvoiceRepository.FindByIdAsync(request.Id, cancellationToken);
            if (existingCosmosInvoice is null)
            {
                throw new NotFoundException($"Could not find CosmosInvoice '{request.Id}'");
            }

            _cosmosInvoiceRepository.Remove(existingCosmosInvoice);
        }
    }

    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeleteCosmosInvoiceCommandValidator : AbstractValidator<DeleteCosmosInvoiceCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteCosmosInvoiceCommandValidator()
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