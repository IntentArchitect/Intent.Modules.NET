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

namespace CleanArchitecture.SingleFiles.Application.AdvancedMappingMongoInvoices
{
    public class DeleteMongoInvoiceCommand : IRequest, ICommand
    {
        private readonly IMongoInvoiceRepository _mongoInvoiceRepository;

        public DeleteMongoInvoiceCommand(string id, IMongoInvoiceRepository mongoInvoiceRepository)
        {
            Id = id;
            _mongoInvoiceRepository = mongoInvoiceRepository;
        }

        public string Id { get; set; }
    }

    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteMongoInvoiceCommandHandler : IRequestHandler<DeleteMongoInvoiceCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteMongoInvoiceCommandHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(DeleteMongoInvoiceCommand request, CancellationToken cancellationToken)
        {
            var mongoInvoice = await _mongoInvoiceRepository.FindByIdAsync(request.Id, cancellationToken);
            if (mongoInvoice is null)
            {
                throw new NotFoundException($"Could not find MongoInvoice '{request.Id}'");
            }

            _mongoInvoiceRepository.Remove(mongoInvoice);
        }
    }

    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeleteMongoInvoiceCommandValidator : AbstractValidator<DeleteMongoInvoiceCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteMongoInvoiceCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Id)
                .NotNull();
        }
    }
}