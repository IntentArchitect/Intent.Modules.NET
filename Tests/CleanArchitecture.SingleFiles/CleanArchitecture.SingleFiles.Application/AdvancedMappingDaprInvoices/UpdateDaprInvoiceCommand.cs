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
    public class UpdateDaprInvoiceCommand : IRequest, ICommand
    {

        public UpdateDaprInvoiceCommand(string description, string id)
        {
            Description = description;
            Id = id;
        }

        public string Description { get; set; }
        public string Id { get; set; }
    }

    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateDaprInvoiceCommandHandler : IRequestHandler<UpdateDaprInvoiceCommand>
    {
        private readonly IDaprInvoiceRepository _daprInvoiceRepository;
        [IntentManaged(Mode.Merge)]
        public UpdateDaprInvoiceCommandHandler(IDaprInvoiceRepository daprInvoiceRepository)
        {
            _daprInvoiceRepository = daprInvoiceRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateDaprInvoiceCommand request, CancellationToken cancellationToken)
        {
            var daprInvoice = await _daprInvoiceRepository.FindByIdAsync(request.Id, cancellationToken);
            if (daprInvoice is null)
            {
                throw new NotFoundException($"Could not find DaprInvoice '{request.Id}'");
            }

            daprInvoice.Description = request.Description;

            _daprInvoiceRepository.Update(daprInvoice);
        }
    }

    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateDaprInvoiceCommandValidator : AbstractValidator<UpdateDaprInvoiceCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateDaprInvoiceCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Description)
                .NotNull();

            RuleFor(v => v.Id)
                .NotNull();
        }
    }
}