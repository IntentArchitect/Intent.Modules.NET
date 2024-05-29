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

namespace CleanArchitecture.SingleFiles.Application.AdvancedMappingEfInvoices
{
    public class UpdateEfInvoiceCommand : IRequest, ICommand
    {

        public UpdateEfInvoiceCommand(string description, Guid id)
        {
            Description = description;
            Id = id;
        }

        public string Description { get; set; }
        public Guid Id { get; set; }
    }

    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateEfInvoiceCommandHandler : IRequestHandler<UpdateEfInvoiceCommand>
    {
        private readonly IEfInvoiceRepository _efInvoiceRepository;
        [IntentManaged(Mode.Merge)]
        public UpdateEfInvoiceCommandHandler(IEfInvoiceRepository efInvoiceRepository)
        {
            _efInvoiceRepository = efInvoiceRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateEfInvoiceCommand request, CancellationToken cancellationToken)
        {
            var efInvoice = await _efInvoiceRepository.FindByIdAsync(request.Id, cancellationToken);
            if (efInvoice is null)
            {
                throw new NotFoundException($"Could not find EfInvoice '{request.Id}'");
            }

            efInvoice.Description = request.Description;
        }
    }

    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateEfInvoiceCommandValidator : AbstractValidator<UpdateEfInvoiceCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateEfInvoiceCommandValidator()
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