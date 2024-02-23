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

namespace CleanArchitecture.SingleFiles.Application.MongoInvoices
{
    public class CreateMongoInvoiceCommand : IRequest<string>, ICommand
    {
        public CreateMongoInvoiceCommand(string description)
        {
            Description = description;
        }

        public string Description { get; set; }
    }

    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateMongoInvoiceCommandHandler : IRequestHandler<CreateMongoInvoiceCommand, string>
    {
        private readonly IMongoInvoiceRepository _mongoInvoiceRepository;
        private readonly IEventBus _eventBus;

        [IntentManaged(Mode.Merge)]
        public CreateMongoInvoiceCommandHandler(IMongoInvoiceRepository mongoInvoiceRepository, IEventBus eventBus)
        {
            _mongoInvoiceRepository = mongoInvoiceRepository;
            _eventBus = eventBus;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<string> Handle(CreateMongoInvoiceCommand request, CancellationToken cancellationToken)
        {
            var newMongoInvoice = new MongoInvoice
            {
                Description = request.Description,
            };

            _mongoInvoiceRepository.Add(newMongoInvoice);
            await _mongoInvoiceRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            _eventBus.Publish(newMongoInvoice.MapToMongoInvoiceCreatedEvent());
            return newMongoInvoice.Id;
        }
    }

    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateMongoInvoiceCommandValidator : AbstractValidator<CreateMongoInvoiceCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateMongoInvoiceCommandValidator()
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