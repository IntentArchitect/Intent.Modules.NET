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

namespace CleanArchitecture.SingleFiles.Application.MongoInvoices
{
    public class UpdateMongoInvoiceCommand : IRequest, ICommand
    {
        public UpdateMongoInvoiceCommand(string id, string description)
        {
            Id = id;
            Description = description;
        }

        public string Id { get; set; }
        public string Description { get; set; }
    }

    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateMongoInvoiceCommandHandler : IRequestHandler<UpdateMongoInvoiceCommand>
    {
        private readonly IMongoInvoiceRepository _mongoInvoiceRepository;
        private readonly IEventBus _eventBus;

        [IntentManaged(Mode.Merge)]
        public UpdateMongoInvoiceCommandHandler(IMongoInvoiceRepository mongoInvoiceRepository, IEventBus eventBus)
        {
            _mongoInvoiceRepository = mongoInvoiceRepository;
            _eventBus = eventBus;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateMongoInvoiceCommand request, CancellationToken cancellationToken)
        {
            var existingMongoInvoice = await _mongoInvoiceRepository.FindByIdAsync(request.Id, cancellationToken);
            if (existingMongoInvoice is null)
            {
                throw new NotFoundException($"Could not find MongoInvoice '{request.Id}'");
            }

            existingMongoInvoice.Description = request.Description;

            _mongoInvoiceRepository.Update(existingMongoInvoice);
            _eventBus.Publish(existingMongoInvoice.MapToMongoInvoiceUpdatedEvent());
        }
    }

    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateMongoInvoiceCommandValidator : AbstractValidator<UpdateMongoInvoiceCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateMongoInvoiceCommandValidator()
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