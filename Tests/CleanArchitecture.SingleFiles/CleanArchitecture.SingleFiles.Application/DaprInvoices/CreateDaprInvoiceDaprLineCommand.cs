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
    public class CreateDaprInvoiceDaprLineCommand : IRequest<string>, ICommand
    {
        public CreateDaprInvoiceDaprLineCommand(string daprInvoiceId, string name)
        {
            DaprInvoiceId = daprInvoiceId;
            Name = name;
        }

        public string DaprInvoiceId { get; set; }
        public string Name { get; set; }
    }

    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateDaprInvoiceDaprLineCommandHandler : IRequestHandler<CreateDaprInvoiceDaprLineCommand, string>
    {
        private readonly IDaprInvoiceRepository _daprInvoiceRepository;

        [IntentManaged(Mode.Merge)]
        public CreateDaprInvoiceDaprLineCommandHandler(IDaprInvoiceRepository daprInvoiceRepository)
        {
            _daprInvoiceRepository = daprInvoiceRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<string> Handle(CreateDaprInvoiceDaprLineCommand request, CancellationToken cancellationToken)
        {
            var aggregateRoot = await _daprInvoiceRepository.FindByIdAsync(request.DaprInvoiceId, cancellationToken);
            if (aggregateRoot is null)
            {
                throw new NotFoundException($"{nameof(DaprInvoice)} of Id '{request.DaprInvoiceId}' could not be found");
            }

            var newDaprLine = new DaprLine
            {
#warning No matching field found for DaprInvoiceId
                Name = request.Name,
            };

            aggregateRoot.DaprLines.Add(newDaprLine);
            _daprInvoiceRepository.Update(aggregateRoot);
            await _daprInvoiceRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return newDaprLine.Id;
        }
    }

    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateDaprInvoiceDaprLineCommandValidator : AbstractValidator<CreateDaprInvoiceDaprLineCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateDaprInvoiceDaprLineCommandValidator()
        {
            // IntentFully(Match = "ConfigureValidationRules")
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.DaprInvoiceId)
                .NotNull();

            RuleFor(v => v.Name)
                .NotNull();
        }
    }
}