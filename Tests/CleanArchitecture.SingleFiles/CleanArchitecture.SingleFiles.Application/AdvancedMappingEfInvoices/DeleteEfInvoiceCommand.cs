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
    public class DeleteEfInvoiceCommand : IRequest, ICommand
    {
        private readonly IEfInvoiceRepository _efInvoiceRepository;

        public DeleteEfInvoiceCommand(Guid id, IEfInvoiceRepository efInvoiceRepository)
        {
            Id = id;
            _efInvoiceRepository = efInvoiceRepository;
        }

        public Guid Id { get; set; }
    }

    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteEfInvoiceCommandHandler : IRequestHandler<DeleteEfInvoiceCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteEfInvoiceCommandHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(DeleteEfInvoiceCommand request, CancellationToken cancellationToken)
        {
            var efInvoice = await _efInvoiceRepository.FindByIdAsync(request.Id, cancellationToken);
            if (efInvoice is null)
            {
                throw new NotFoundException($"Could not find EfInvoice '{request.Id}'");
            }

            _efInvoiceRepository.Remove(efInvoice);
        }
    }

    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeleteEfInvoiceCommandValidator : AbstractValidator<DeleteEfInvoiceCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteEfInvoiceCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}