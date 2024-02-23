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

namespace CleanArchitecture.SingleFiles.Application.DaprInvoices
{
    public class UpdateDaprInvoiceCommand : IRequest, ICommand
    {
        public UpdateDaprInvoiceCommand(string id, string description)
        {
            Id = id;
            Description = description;
        }

        public string Id { get; set; }
        public string Description { get; set; }
    }

    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateDaprInvoiceCommandHandler : IRequestHandler<UpdateDaprInvoiceCommand>
    {
        private readonly IDaprInvoiceRepository _daprInvoiceRepository;
        private readonly IEventBus _eventBus;

        [IntentManaged(Mode.Merge)]
        public UpdateDaprInvoiceCommandHandler(IDaprInvoiceRepository daprInvoiceRepository, IEventBus eventBus)
        {
            _daprInvoiceRepository = daprInvoiceRepository;
            _eventBus = eventBus;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateDaprInvoiceCommand request, CancellationToken cancellationToken)
        {
            var existingDaprInvoice = await _daprInvoiceRepository.FindByIdAsync(request.Id, cancellationToken);
            if (existingDaprInvoice is null)
            {
                throw new NotFoundException($"Could not find DaprInvoice '{request.Id}'");
            }

            existingDaprInvoice.Description = request.Description;

            _daprInvoiceRepository.Update(existingDaprInvoice);
            _eventBus.Publish(existingDaprInvoice.MapToDaprInvoiceUpdatedEvent());
        }
    }

    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateDaprInvoiceCommandValidator : AbstractValidator<UpdateDaprInvoiceCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateDaprInvoiceCommandValidator()
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