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

namespace CleanArchitecture.SingleFiles.Application.AdvancedMappingCosmosInvoices
{
    public class CreateCosmosInvoiceCommand : IRequest<string>, ICommand
    {

        public CreateCosmosInvoiceCommand(string description)
        {
            Description = description;
        }

        public string Description { get; set; }
    }

    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateCosmosInvoiceCommandHandler : IRequestHandler<CreateCosmosInvoiceCommand, string>
    {
        private readonly ICosmosInvoiceRepository _cosmosInvoiceRepository;
        [IntentManaged(Mode.Merge)]
        public CreateCosmosInvoiceCommandHandler(ICosmosInvoiceRepository cosmosInvoiceRepository)
        {
            _cosmosInvoiceRepository = cosmosInvoiceRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<string> Handle(CreateCosmosInvoiceCommand request, CancellationToken cancellationToken)
        {
            var cosmosInvoice = new CosmosInvoice
            {
                Description = request.Description
            };

            _cosmosInvoiceRepository.Add(cosmosInvoice);
            await _cosmosInvoiceRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return cosmosInvoice.Id;
        }
    }

    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateCosmosInvoiceCommandValidator : AbstractValidator<CreateCosmosInvoiceCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateCosmosInvoiceCommandValidator()
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