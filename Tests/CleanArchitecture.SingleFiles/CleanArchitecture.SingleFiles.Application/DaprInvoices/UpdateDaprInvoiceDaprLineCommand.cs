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
    public class UpdateDaprInvoiceDaprLineCommand : IRequest, ICommand
    {
        public UpdateDaprInvoiceDaprLineCommand(string daprInvoiceId, string id, string name)
        {
            DaprInvoiceId = daprInvoiceId;
            Id = id;
            Name = name;
        }

        public string DaprInvoiceId { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
    }

    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateDaprInvoiceDaprLineCommandHandler : IRequestHandler<UpdateDaprInvoiceDaprLineCommand>
    {
        private readonly IDaprInvoiceRepository _daprInvoiceRepository;

        [IntentManaged(Mode.Merge)]
        public UpdateDaprInvoiceDaprLineCommandHandler(IDaprInvoiceRepository daprInvoiceRepository)
        {
            _daprInvoiceRepository = daprInvoiceRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateDaprInvoiceDaprLineCommand request, CancellationToken cancellationToken)
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

#warning No matching field found for DaprInvoiceId
            existingDaprLine.Name = request.Name;

            _daprInvoiceRepository.Update(aggregateRoot);
        }
    }

    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateDaprInvoiceDaprLineCommandValidator : AbstractValidator<UpdateDaprInvoiceDaprLineCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateDaprInvoiceDaprLineCommandValidator()
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

            RuleFor(v => v.Name)
                .NotNull();
        }
    }
}