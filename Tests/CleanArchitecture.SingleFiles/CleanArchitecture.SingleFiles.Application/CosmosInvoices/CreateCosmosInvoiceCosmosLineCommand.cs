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

namespace CleanArchitecture.SingleFiles.Application.CosmosInvoices
{
    public class CreateCosmosInvoiceCosmosLineCommand : IRequest<string>, ICommand
    {
        public CreateCosmosInvoiceCosmosLineCommand(string cosmosInvoiceId, string name)
        {
            CosmosInvoiceId = cosmosInvoiceId;
            Name = name;
        }

        public string CosmosInvoiceId { get; set; }
        public string Name { get; set; }
    }

    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateCosmosInvoiceCosmosLineCommandHandler : IRequestHandler<CreateCosmosInvoiceCosmosLineCommand, string>
    {
        private readonly ICosmosInvoiceRepository _cosmosInvoiceRepository;

        [IntentManaged(Mode.Merge)]
        public CreateCosmosInvoiceCosmosLineCommandHandler(ICosmosInvoiceRepository cosmosInvoiceRepository)
        {
            _cosmosInvoiceRepository = cosmosInvoiceRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<string> Handle(CreateCosmosInvoiceCosmosLineCommand request, CancellationToken cancellationToken)
        {
            var aggregateRoot = await _cosmosInvoiceRepository.FindByIdAsync(request.CosmosInvoiceId, cancellationToken);
            if (aggregateRoot is null)
            {
                throw new NotFoundException($"{nameof(CosmosInvoice)} of Id '{request.CosmosInvoiceId}' could not be found");
            }

            var newCosmosLine = new CosmosLine
            {
#warning No matching field found for CosmosInvoiceId
                Name = request.Name,
            };

            aggregateRoot.CosmosLines.Add(newCosmosLine);
            _cosmosInvoiceRepository.Update(aggregateRoot);
            await _cosmosInvoiceRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return newCosmosLine.Id;
        }
    }

    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateCosmosInvoiceCosmosLineCommandValidator : AbstractValidator<CreateCosmosInvoiceCosmosLineCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateCosmosInvoiceCosmosLineCommandValidator()
        {
            // IntentFully(Match = "ConfigureValidationRules")
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.CosmosInvoiceId)
                .NotNull();

            RuleFor(v => v.Name)
                .NotNull();
        }
    }
}