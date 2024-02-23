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

namespace CleanArchitecture.SingleFiles.Application.EfInvoices
{
    public class UpdateEfInvoiceCommand : IRequest, ICommand
    {
        public UpdateEfInvoiceCommand(Guid id, string description)
        {
            Id = id;
            Description = description;
        }

        public Guid Id { get; set; }
        public string Description { get; set; }
    }

    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateEfInvoiceCommandHandler : IRequestHandler<UpdateEfInvoiceCommand>
    {
        private readonly IEfInvoiceRepository _efInvoiceRepository;
        private readonly IEventBus _eventBus;

        [IntentManaged(Mode.Merge)]
        public UpdateEfInvoiceCommandHandler(IEfInvoiceRepository efInvoiceRepository, IEventBus eventBus)
        {
            _efInvoiceRepository = efInvoiceRepository;
            _eventBus = eventBus;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateEfInvoiceCommand request, CancellationToken cancellationToken)
        {
            var existingEfInvoice = await _efInvoiceRepository.FindByIdAsync(request.Id, cancellationToken);
            if (existingEfInvoice is null)
            {
                throw new NotFoundException($"Could not find EfInvoice '{request.Id}'");
            }

            existingEfInvoice.Description = request.Description;
            _eventBus.Publish(existingEfInvoice.MapToEfInvoiceUpdatedEvent());
        }
    }

    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateEfInvoiceCommandValidator : AbstractValidator<UpdateEfInvoiceCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateEfInvoiceCommandValidator()
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