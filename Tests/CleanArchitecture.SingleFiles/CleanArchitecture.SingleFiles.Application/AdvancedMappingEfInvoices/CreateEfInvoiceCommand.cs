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
        private readonly IEfInvoiceRepository _efInvoiceRepository;

        public CreateEfInvoiceCommand(string description, IEfInvoiceRepository efInvoiceRepository)
        {
            Description = description;
            _efInvoiceRepository = efInvoiceRepository;
        }

        public string Description { get; set; }
    }

    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateEfInvoiceCommandHandler : IRequestHandler<CreateEfInvoiceCommand, Guid>
    {
        [IntentManaged(Mode.Merge)]
        public CreateEfInvoiceCommandHandler()
        {
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