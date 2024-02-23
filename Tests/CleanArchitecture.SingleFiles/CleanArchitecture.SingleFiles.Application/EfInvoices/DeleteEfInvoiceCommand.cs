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

namespace CleanArchitecture.SingleFiles.Application.EfInvoices
{
    public class DeleteEfInvoiceCommand : IRequest, ICommand
    {
        public DeleteEfInvoiceCommand(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }

    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteEfInvoiceCommandHandler : IRequestHandler<DeleteEfInvoiceCommand>
    {
        private readonly IEfInvoiceRepository _efInvoiceRepository;

        [IntentManaged(Mode.Merge)]
        public DeleteEfInvoiceCommandHandler(IEfInvoiceRepository efInvoiceRepository)
        {
            _efInvoiceRepository = efInvoiceRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(DeleteEfInvoiceCommand request, CancellationToken cancellationToken)
        {
            var existingEfInvoice = await _efInvoiceRepository.FindByIdAsync(request.Id, cancellationToken);
            if (existingEfInvoice is null)
            {
                throw new NotFoundException($"Could not find EfInvoice '{request.Id}'");
            }

            _efInvoiceRepository.Remove(existingEfInvoice);
        }
    }

    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeleteEfInvoiceCommandValidator : AbstractValidator<DeleteEfInvoiceCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteEfInvoiceCommandValidator()
        {
            // IntentFully(Match = "ConfigureValidationRules")
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}