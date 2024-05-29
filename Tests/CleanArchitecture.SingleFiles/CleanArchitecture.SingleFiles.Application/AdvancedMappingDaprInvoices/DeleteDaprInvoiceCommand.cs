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

namespace CleanArchitecture.SingleFiles.Application.AdvancedMappingDaprInvoices
{
    public class DeleteDaprInvoiceCommand : IRequest, ICommand
    {
        private readonly IDaprInvoiceRepository _daprInvoiceRepository;

        public DeleteDaprInvoiceCommand(string id, IDaprInvoiceRepository daprInvoiceRepository)
        {
            Id = id;
            _daprInvoiceRepository = daprInvoiceRepository;
        }

        public string Id { get; set; }
    }

    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteDaprInvoiceCommandHandler : IRequestHandler<DeleteDaprInvoiceCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteDaprInvoiceCommandHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(DeleteDaprInvoiceCommand request, CancellationToken cancellationToken)
        {
            var daprInvoice = await _daprInvoiceRepository.FindByIdAsync(request.Id, cancellationToken);
            if (daprInvoice is null)
            {
                throw new NotFoundException($"Could not find DaprInvoice '{request.Id}'");
            }

            _daprInvoiceRepository.Remove(daprInvoice);
        }
    }

    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeleteDaprInvoiceCommandValidator : AbstractValidator<DeleteDaprInvoiceCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteDaprInvoiceCommandValidator()
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