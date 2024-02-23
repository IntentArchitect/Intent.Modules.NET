using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.SingleFiles.Application.Common.Eventing;
using CleanArchitecture.SingleFiles.Application.Common.Interfaces;
using CleanArchitecture.SingleFiles.Domain.Common.Exceptions;
using CleanArchitecture.SingleFiles.Domain.Repositories;
using CleanArchitecture.SingleFiles.Messages;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.SingleFiles.Application.CosmosInvoices
{
    public class UpdateCosmosInvoiceCommand : IRequest, ICommand
    {
        public UpdateCosmosInvoiceCommand(string id, string description)
        {
            Id = id;
            Description = description;
        }

        public string Id { get; set; }
        public string Description { get; set; }
    }

    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateCosmosInvoiceCommandHandler : IRequestHandler<UpdateCosmosInvoiceCommand>
    {
        private readonly ICosmosInvoiceRepository _cosmosInvoiceRepository;
        private readonly IEventBus _eventBus;

        [IntentManaged(Mode.Merge)]
        public UpdateCosmosInvoiceCommandHandler(ICosmosInvoiceRepository cosmosInvoiceRepository, IEventBus eventBus)
        {
            _cosmosInvoiceRepository = cosmosInvoiceRepository;
            _eventBus = eventBus;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateCosmosInvoiceCommand request, CancellationToken cancellationToken)
        {
            var existingCosmosInvoice = await _cosmosInvoiceRepository.FindByIdAsync(request.Id, cancellationToken);
            if (existingCosmosInvoice is null)
            {
                throw new NotFoundException($"Could not find CosmosInvoice '{request.Id}'");
            }

            existingCosmosInvoice.Description = request.Description;

            _cosmosInvoiceRepository.Update(existingCosmosInvoice);
            _eventBus.Publish(existingCosmosInvoice.MapToCosmosInvoiceUpdatedEvent());
        }
    }

    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateCosmosInvoiceCommandValidator : AbstractValidator<UpdateCosmosInvoiceCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateCosmosInvoiceCommandValidator()
        {
            // IntentFully(Match = "ConfigureValidationRules")
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Id)
                .NotNull();

            RuleFor(v => v.Description)
                .NotNull();
        }
    }
}