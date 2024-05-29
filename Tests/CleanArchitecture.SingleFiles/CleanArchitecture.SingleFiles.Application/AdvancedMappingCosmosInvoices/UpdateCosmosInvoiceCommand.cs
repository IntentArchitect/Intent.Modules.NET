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

namespace CleanArchitecture.SingleFiles.Application.AdvancedMappingCosmosInvoices
{
    public class UpdateCosmosInvoiceCommand : IRequest, ICommand
    {
        private readonly ICosmosInvoiceRepository _cosmosInvoiceRepository;

        public UpdateCosmosInvoiceCommand(string id, string description, ICosmosInvoiceRepository cosmosInvoiceRepository)
        {
            Id = id;
            Description = description;
            _cosmosInvoiceRepository = cosmosInvoiceRepository;
        }

        public string Id { get; set; }
        public string Description { get; set; }
    }

    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateCosmosInvoiceCommandHandler : IRequestHandler<UpdateCosmosInvoiceCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateCosmosInvoiceCommandHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateCosmosInvoiceCommand request, CancellationToken cancellationToken)
        {
            var cosmosInvoice = await _cosmosInvoiceRepository.FindByIdAsync(request.Id, cancellationToken);
            if (cosmosInvoice is null)
            {
                throw new NotFoundException($"Could not find CosmosInvoice '{request.Id}'");
            }

            cosmosInvoice.Description = request.Description;

            _cosmosInvoiceRepository.Update(cosmosInvoice);
        }
    }

    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateCosmosInvoiceCommandValidator : AbstractValidator<UpdateCosmosInvoiceCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateCosmosInvoiceCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Id)
                .NotNull();

            RuleFor(v => v.Description)
                .NotNull();
        }
    }
}