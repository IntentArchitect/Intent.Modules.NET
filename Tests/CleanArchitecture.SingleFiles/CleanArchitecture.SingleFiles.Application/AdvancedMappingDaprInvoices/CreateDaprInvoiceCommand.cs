using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.SingleFiles.Application.Common.Interfaces;
using CleanArchitecture.SingleFiles.Domain.Entities;
using CleanArchitecture.SingleFiles.Domain.Repositories;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.SingleFiles.Application.AdvancedMappingDaprInvoices
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
        [IntentManaged(Mode.Merge)]
        public CreateDaprInvoiceCommandHandler(IDaprInvoiceRepository daprInvoiceRepository)
        {
            _daprInvoiceRepository = daprInvoiceRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<string> Handle(CreateDaprInvoiceCommand request, CancellationToken cancellationToken)
        {
            var daprInvoice = new DaprInvoice
            {
                Description = request.Description
            };

            _daprInvoiceRepository.Add(daprInvoice);
            await _daprInvoiceRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return daprInvoice.Id;
        }
    }

    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateDaprInvoiceCommandValidator : AbstractValidator<CreateDaprInvoiceCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateDaprInvoiceCommandValidator()
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