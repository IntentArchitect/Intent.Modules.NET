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

namespace CleanArchitecture.SingleFiles.Application.EfInvoices
{
    public class CreateEfInvoiceCommand : IRequest<Guid>, ICommand
    {
        public CreateEfInvoiceCommand(string description)
        {
            Description = description;
        }

        public string Description { get; set; }
    }

    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateEfInvoiceCommandHandler : IRequestHandler<CreateEfInvoiceCommand, Guid>
    {
        private readonly IEfInvoiceRepository _efInvoiceRepository;
        private readonly IEventBus _eventBus;

        [IntentManaged(Mode.Merge)]
        public CreateEfInvoiceCommandHandler(IEfInvoiceRepository efInvoiceRepository, IEventBus eventBus)
        {
            _efInvoiceRepository = efInvoiceRepository;
            _eventBus = eventBus;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateEfInvoiceCommand request, CancellationToken cancellationToken)
        {
            var newEfInvoice = new EfInvoice
            {
                Description = request.Description,
            };

            _efInvoiceRepository.Add(newEfInvoice);
            await _efInvoiceRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            _eventBus.Publish(new EfInvoiceCreatedEvent
            {
                Description = request.Description
            });
            return newEfInvoice.Id;
        }
    }

    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateEfInvoiceCommandValidator : AbstractValidator<CreateEfInvoiceCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateEfInvoiceCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Description)
                .NotNull();
        }
    }
}