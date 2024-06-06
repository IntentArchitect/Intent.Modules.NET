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

namespace CleanArchitecture.SingleFiles.Application.AdvancedMappingMongoInvoices
{
    public class CreateMongoInvoiceCommand : IRequest<string>, ICommand
    {

        public CreateMongoInvoiceCommand(string description)
        {
            Description = description;
        }

        public string Description { get; set; }
    }

    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateMongoInvoiceCommandHandler : IRequestHandler<CreateMongoInvoiceCommand, string>
    {
        private readonly IMongoInvoiceRepository _mongoInvoiceRepository;
        [IntentManaged(Mode.Merge)]
        public CreateMongoInvoiceCommandHandler(IMongoInvoiceRepository mongoInvoiceRepository)
        {
            _mongoInvoiceRepository = mongoInvoiceRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<string> Handle(CreateMongoInvoiceCommand request, CancellationToken cancellationToken)
        {
            var mongoInvoice = new MongoInvoice
            {
                Description = request.Description
            };

            _mongoInvoiceRepository.Add(mongoInvoice);
            await _mongoInvoiceRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return mongoInvoice.Id;
        }
    }

    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateMongoInvoiceCommandValidator : AbstractValidator<CreateMongoInvoiceCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateMongoInvoiceCommandValidator()
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