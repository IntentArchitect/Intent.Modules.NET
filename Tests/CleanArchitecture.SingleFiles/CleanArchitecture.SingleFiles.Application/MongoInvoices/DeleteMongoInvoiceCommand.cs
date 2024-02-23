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

namespace CleanArchitecture.SingleFiles.Application.MongoInvoices
{
    public class DeleteMongoInvoiceCommand : IRequest, ICommand
    {
        public DeleteMongoInvoiceCommand(string id)
        {
            Id = id;
        }

        public string Id { get; set; }
    }

    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteMongoInvoiceCommandHandler : IRequestHandler<DeleteMongoInvoiceCommand>
    {
        private readonly IMongoInvoiceRepository _mongoInvoiceRepository;

        [IntentManaged(Mode.Merge)]
        public DeleteMongoInvoiceCommandHandler(IMongoInvoiceRepository mongoInvoiceRepository)
        {
            _mongoInvoiceRepository = mongoInvoiceRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(DeleteMongoInvoiceCommand request, CancellationToken cancellationToken)
        {
            var existingMongoInvoice = await _mongoInvoiceRepository.FindByIdAsync(request.Id, cancellationToken);
            if (existingMongoInvoice is null)
            {
                throw new NotFoundException($"Could not find MongoInvoice '{request.Id}'");
            }

            _mongoInvoiceRepository.Remove(existingMongoInvoice);
        }
    }

    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeleteMongoInvoiceCommandValidator : AbstractValidator<DeleteMongoInvoiceCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteMongoInvoiceCommandValidator()
        {
            // IntentFully(Match = "ConfigureValidationRules")
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Id)
                .NotNull();
        }
    }
}