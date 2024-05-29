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

namespace CleanArchitecture.SingleFiles.Application.AdvancedMappingEfInvoices
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
        [IntentManaged(Mode.Merge)]
        public CreateEfInvoiceCommandHandler(IEfInvoiceRepository efInvoiceRepository)
        {
            _efInvoiceRepository = efInvoiceRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateEfInvoiceCommand request, CancellationToken cancellationToken)
        {
            var efInvoice = new EfInvoice
            {
                Description = request.Description
            };

            _efInvoiceRepository.Add(efInvoice);
            await _efInvoiceRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return efInvoice.Id;
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