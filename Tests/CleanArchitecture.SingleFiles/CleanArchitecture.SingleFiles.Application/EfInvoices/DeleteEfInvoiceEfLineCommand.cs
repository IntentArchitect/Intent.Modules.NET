using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.SingleFiles.Application.Common.Interfaces;
using CleanArchitecture.SingleFiles.Domain.Common.Exceptions;
using CleanArchitecture.SingleFiles.Domain.Entities;
using CleanArchitecture.SingleFiles.Domain.Repositories;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.SingleFiles.Application.EfInvoices
{
    public class DeleteEfInvoiceEfLineCommand : IRequest, ICommand
    {
        public DeleteEfInvoiceEfLineCommand(Guid efInvoicesId, Guid id)
        {
            EfInvoicesId = efInvoicesId;
            Id = id;
        }

        public Guid EfInvoicesId { get; set; }
        public Guid Id { get; set; }
    }

    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteEfInvoiceEfLineCommandHandler : IRequestHandler<DeleteEfInvoiceEfLineCommand>
    {
        private readonly IEfInvoiceRepository _efInvoiceRepository;

        [IntentManaged(Mode.Merge)]
        public DeleteEfInvoiceEfLineCommandHandler(IEfInvoiceRepository efInvoiceRepository)
        {
            _efInvoiceRepository = efInvoiceRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(DeleteEfInvoiceEfLineCommand request, CancellationToken cancellationToken)
        {
            var aggregateRoot = await _efInvoiceRepository.FindByIdAsync(request.EfInvoicesId, cancellationToken);
            if (aggregateRoot is null)
            {
                throw new NotFoundException($"{nameof(EfInvoice)} of Id '{request.EfInvoicesId}' could not be found");
            }

            var existingEfLine = aggregateRoot.EfLines.FirstOrDefault(p => p.Id == request.Id);
            if (existingEfLine is null)
            {
                throw new NotFoundException($"{nameof(EfLine)} of Id '{request.Id}' could not be found associated with {nameof(EfInvoice)} of Id '{request.EfInvoicesId}'");
            }

            aggregateRoot.EfLines.Remove(existingEfLine);
        }
    }

    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeleteEfInvoiceEfLineCommandValidator : AbstractValidator<DeleteEfInvoiceEfLineCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteEfInvoiceEfLineCommandValidator()
        {
            // IntentFully(Match = "ConfigureValidationRules")
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}