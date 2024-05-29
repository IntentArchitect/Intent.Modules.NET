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
    public class UpdateMongoInvoiceCommand : IRequest, ICommand
    {
        private readonly IMongoInvoiceRepository _mongoInvoiceRepository;

        public UpdateMongoInvoiceCommand(string description, string id, IMongoInvoiceRepository mongoInvoiceRepository)
        {
            Description = description;
            Id = id;
            _mongoInvoiceRepository = mongoInvoiceRepository;
        }

        public string Description { get; set; }
        public string Id { get; set; }
    }

    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateMongoInvoiceCommandHandler : IRequestHandler<UpdateMongoInvoiceCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateMongoInvoiceCommandHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateMongoInvoiceCommand request, CancellationToken cancellationToken)
        {
            var mongoInvoice = await _mongoInvoiceRepository.FindByIdAsync(request.Id, cancellationToken);
            if (mongoInvoice is null)
            {
                throw new NotFoundException($"Could not find MongoInvoice '{request.Id}'");
            }

            mongoInvoice.Description = request.Description;

            _mongoInvoiceRepository.Update(mongoInvoice);
        }
    }

    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateMongoInvoiceCommandValidator : AbstractValidator<UpdateMongoInvoiceCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateMongoInvoiceCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Description)
                .NotNull();

            RuleFor(v => v.Id)
                .NotNull();
        }
    }
}