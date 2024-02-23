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

namespace CleanArchitecture.SingleFiles.Application.DaprInvoices
{
    public class DeleteDaprInvoiceDaprLineCommand : IRequest, ICommand
    {
        public DeleteDaprInvoiceDaprLineCommand(string daprInvoiceId, string id)
        {
            DaprInvoiceId = daprInvoiceId;
            Id = id;
        }

        public string DaprInvoiceId { get; set; }
        public string Id { get; set; }
    }

    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteDaprInvoiceDaprLineCommandHandler : IRequestHandler<DeleteDaprInvoiceDaprLineCommand>
    {
        private readonly IDaprInvoiceRepository _daprInvoiceRepository;

        [IntentManaged(Mode.Merge)]
        public DeleteDaprInvoiceDaprLineCommandHandler(IDaprInvoiceRepository daprInvoiceRepository)
        {
            _daprInvoiceRepository = daprInvoiceRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(DeleteDaprInvoiceDaprLineCommand request, CancellationToken cancellationToken)
        {
            var aggregateRoot = await _daprInvoiceRepository.FindByIdAsync(request.DaprInvoiceId, cancellationToken);
            if (aggregateRoot is null)
            {
                throw new NotFoundException($"{nameof(DaprInvoice)} of Id '{request.DaprInvoiceId}' could not be found");
            }

            var existingDaprLine = aggregateRoot.DaprLines.FirstOrDefault(p => p.Id == request.Id);
            if (existingDaprLine is null)
            {
                throw new NotFoundException($"{nameof(DaprLine)} of Id '{request.Id}' could not be found associated with {nameof(DaprInvoice)} of Id '{request.DaprInvoiceId}'");
            }

            aggregateRoot.DaprLines.Remove(existingDaprLine);

            _daprInvoiceRepository.Update(aggregateRoot);
        }
    }

    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeleteDaprInvoiceDaprLineCommandValidator : AbstractValidator<DeleteDaprInvoiceDaprLineCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteDaprInvoiceDaprLineCommandValidator()
        {
            // IntentFully(Match = "ConfigureValidationRules")
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.DaprInvoiceId)
                .NotNull();

            RuleFor(v => v.Id)
                .NotNull();
        }
    }
}