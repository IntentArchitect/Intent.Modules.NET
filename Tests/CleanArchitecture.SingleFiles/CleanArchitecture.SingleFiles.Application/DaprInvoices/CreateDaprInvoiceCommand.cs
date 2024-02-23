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

namespace CleanArchitecture.SingleFiles.Application.DaprInvoices
{
    public class CreateDaprInvoiceCommand : IRequest<string>, ICommand
    {
        public CreateDaprInvoiceCommand(string description)
        {
            Description = description;
        }

        public string Description { get; set; }
    }

    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateDaprInvoiceCommandHandler : IRequestHandler<CreateDaprInvoiceCommand, string>
    {
        private readonly IDaprInvoiceRepository _daprInvoiceRepository;
        private readonly IEventBus _eventBus;

        [IntentManaged(Mode.Merge)]
        public CreateDaprInvoiceCommandHandler(IDaprInvoiceRepository daprInvoiceRepository, IEventBus eventBus)
        {
            _daprInvoiceRepository = daprInvoiceRepository;
            _eventBus = eventBus;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<string> Handle(CreateDaprInvoiceCommand request, CancellationToken cancellationToken)
        {
            var newDaprInvoice = new DaprInvoice
            {
                Description = request.Description,
            };

            _daprInvoiceRepository.Add(newDaprInvoice);
            await _daprInvoiceRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            _eventBus.Publish(newDaprInvoice.MapToDaprInvoiceCreatedEvent());
            return newDaprInvoice.Id;
        }
    }

    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateDaprInvoiceCommandValidator : AbstractValidator<CreateDaprInvoiceCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateDaprInvoiceCommandValidator()
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