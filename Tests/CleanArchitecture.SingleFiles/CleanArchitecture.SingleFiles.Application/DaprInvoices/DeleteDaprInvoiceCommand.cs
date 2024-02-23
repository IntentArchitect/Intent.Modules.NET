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

namespace CleanArchitecture.SingleFiles.Application.DaprInvoices
{
    public class DeleteDaprInvoiceCommand : IRequest, ICommand
    {
        public DeleteDaprInvoiceCommand(string id)
        {
            Id = id;
        }

        public string Id { get; set; }
    }

    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteDaprInvoiceCommandHandler : IRequestHandler<DeleteDaprInvoiceCommand>
    {
        private readonly IDaprInvoiceRepository _daprInvoiceRepository;

        [IntentManaged(Mode.Merge)]
        public DeleteDaprInvoiceCommandHandler(IDaprInvoiceRepository daprInvoiceRepository)
        {
            _daprInvoiceRepository = daprInvoiceRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(DeleteDaprInvoiceCommand request, CancellationToken cancellationToken)
        {
            var existingDaprInvoice = await _daprInvoiceRepository.FindByIdAsync(request.Id, cancellationToken);
            if (existingDaprInvoice is null)
            {
                throw new NotFoundException($"Could not find DaprInvoice '{request.Id}'");
            }

            _daprInvoiceRepository.Remove(existingDaprInvoice);
        }
    }

    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeleteDaprInvoiceCommandValidator : AbstractValidator<DeleteDaprInvoiceCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteDaprInvoiceCommandValidator()
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