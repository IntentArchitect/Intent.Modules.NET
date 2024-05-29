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

namespace CleanArchitecture.SingleFiles.Application.AdvancedMappingCosmosInvoices
{
    public class DeleteCosmosInvoiceCommand : IRequest, ICommand
    {
        private readonly ICosmosInvoiceRepository _cosmosInvoiceRepository;

        public DeleteCosmosInvoiceCommand(string id, ICosmosInvoiceRepository cosmosInvoiceRepository)
        {
            Id = id;
            _cosmosInvoiceRepository = cosmosInvoiceRepository;
        }

        public string Id { get; set; }
    }

    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteCosmosInvoiceCommandHandler : IRequestHandler<DeleteCosmosInvoiceCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteCosmosInvoiceCommandHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(DeleteCosmosInvoiceCommand request, CancellationToken cancellationToken)
        {
            var cosmosInvoice = await _cosmosInvoiceRepository.FindByIdAsync(request.Id, cancellationToken);
            if (cosmosInvoice is null)
            {
                throw new NotFoundException($"Could not find CosmosInvoice '{request.Id}'");
            }

            _cosmosInvoiceRepository.Remove(cosmosInvoice);
        }
    }

    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeleteCosmosInvoiceCommandValidator : AbstractValidator<DeleteCosmosInvoiceCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteCosmosInvoiceCommandValidator()
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