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
    public class CreateEfInvoiceEfLineCommand : IRequest<Guid>, ICommand
    {
        public CreateEfInvoiceEfLineCommand(Guid efInvoicesId, string name)
        {
            EfInvoicesId = efInvoicesId;
            Name = name;
        }

        public Guid EfInvoicesId { get; set; }
        public string Name { get; set; }
    }

    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateEfInvoiceEfLineCommandHandler : IRequestHandler<CreateEfInvoiceEfLineCommand, Guid>
    {
        private readonly IEfInvoiceRepository _efInvoiceRepository;

        [IntentManaged(Mode.Merge)]
        public CreateEfInvoiceEfLineCommandHandler(IEfInvoiceRepository efInvoiceRepository)
        {
            _efInvoiceRepository = efInvoiceRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateEfInvoiceEfLineCommand request, CancellationToken cancellationToken)
        {
            var aggregateRoot = await _efInvoiceRepository.FindByIdAsync(request.EfInvoicesId, cancellationToken);
            if (aggregateRoot is null)
            {
                throw new NotFoundException($"{nameof(EfInvoice)} of Id '{request.EfInvoicesId}' could not be found");
            }

            var newEfLine = new EfLine
            {
                EfInvoicesId = request.EfInvoicesId,
                Name = request.Name,
            };

            aggregateRoot.EfLines.Add(newEfLine);
            await _efInvoiceRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return newEfLine.Id;
        }
    }

    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateEfInvoiceEfLineCommandValidator : AbstractValidator<CreateEfInvoiceEfLineCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateEfInvoiceEfLineCommandValidator()
        {
            // IntentFully(Match = "ConfigureValidationRules")
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Name)
                .NotNull();
        }
    }
}