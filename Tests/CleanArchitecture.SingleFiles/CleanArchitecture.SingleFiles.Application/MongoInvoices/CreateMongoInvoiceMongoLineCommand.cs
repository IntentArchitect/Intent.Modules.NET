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

namespace CleanArchitecture.SingleFiles.Application.MongoInvoices
{
    public class CreateMongoInvoiceMongoLineCommand : IRequest<string>, ICommand
    {
        public CreateMongoInvoiceMongoLineCommand(string mongoInvoiceId, string name)
        {
            MongoInvoiceId = mongoInvoiceId;
            Name = name;
        }

        public string MongoInvoiceId { get; set; }
        public string Name { get; set; }
    }

    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateMongoInvoiceMongoLineCommandHandler : IRequestHandler<CreateMongoInvoiceMongoLineCommand, string>
    {
        private readonly IMongoInvoiceRepository _mongoInvoiceRepository;

        [IntentManaged(Mode.Merge)]
        public CreateMongoInvoiceMongoLineCommandHandler(IMongoInvoiceRepository mongoInvoiceRepository)
        {
            _mongoInvoiceRepository = mongoInvoiceRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<string> Handle(CreateMongoInvoiceMongoLineCommand request, CancellationToken cancellationToken)
        {
            var aggregateRoot = await _mongoInvoiceRepository.FindByIdAsync(request.MongoInvoiceId, cancellationToken);
            if (aggregateRoot is null)
            {
                throw new NotFoundException($"{nameof(MongoInvoice)} of Id '{request.MongoInvoiceId}' could not be found");
            }

            var newMongoLine = new MongoLine
            {
#warning No matching field found for MongoInvoiceId
                Name = request.Name,
            };

            aggregateRoot.MongoLines.Add(newMongoLine);
            _mongoInvoiceRepository.Update(aggregateRoot);
            await _mongoInvoiceRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return newMongoLine.Id;
        }
    }

    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateMongoInvoiceMongoLineCommandValidator : AbstractValidator<CreateMongoInvoiceMongoLineCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateMongoInvoiceMongoLineCommandValidator()
        {
            // IntentFully(Match = "ConfigureValidationRules")
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.MongoInvoiceId)
                .NotNull();

            RuleFor(v => v.Name)
                .NotNull();
        }
    }
}