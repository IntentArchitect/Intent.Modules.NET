using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.SingleFiles.Application.Common.Eventing;
using CleanArchitecture.SingleFiles.Application.Common.Interfaces;
using CleanArchitecture.SingleFiles.Domain.Entities;
using CleanArchitecture.SingleFiles.Domain.Repositories;
using CleanArchitecture.SingleFiles.Messages;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.SingleFiles.Application.CosmosInvoices
{
    public class CreateCosmosInvoiceCommand : IRequest<string>, ICommand
    {
        public CreateCosmosInvoiceCommand(string description)
        {
            Description = description;
        }

        public string Description { get; set; }
    }

    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateCosmosInvoiceCommandHandler : IRequestHandler<CreateCosmosInvoiceCommand, string>
    {
        private readonly ICosmosInvoiceRepository _cosmosInvoiceRepository;
        private readonly IEventBus _eventBus;

        [IntentManaged(Mode.Merge)]
        public CreateCosmosInvoiceCommandHandler(ICosmosInvoiceRepository cosmosInvoiceRepository, IEventBus eventBus)
        {
            _cosmosInvoiceRepository = cosmosInvoiceRepository;
            _eventBus = eventBus;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<string> Handle(CreateCosmosInvoiceCommand request, CancellationToken cancellationToken)
        {
            var newCosmosInvoice = new CosmosInvoice
            {
                Description = request.Description,
            };

            _cosmosInvoiceRepository.Add(newCosmosInvoice);
            await _cosmosInvoiceRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            _eventBus.Publish(newCosmosInvoice.MapToCosmosInvoiceCreatedEvent());
            return newCosmosInvoice.Id;
        }
    }

    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateCosmosInvoiceCommandValidator : AbstractValidator<CreateCosmosInvoiceCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateCosmosInvoiceCommandValidator()
        {
            // IntentFully(Match = "ConfigureValidationRules")
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Description)
                .NotNull();
        }
    }
}