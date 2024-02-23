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
    public class DeleteMongoInvoiceMongoLineCommand : IRequest, ICommand
    {
        public DeleteMongoInvoiceMongoLineCommand(string mongoInvoiceId, string id)
        {
            MongoInvoiceId = mongoInvoiceId;
            Id = id;
        }

        public string MongoInvoiceId { get; set; }
        public string Id { get; set; }
    }

    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteMongoInvoiceMongoLineCommandHandler : IRequestHandler<DeleteMongoInvoiceMongoLineCommand>
    {
        private readonly IMongoInvoiceRepository _mongoInvoiceRepository;

        [IntentManaged(Mode.Merge)]
        public DeleteMongoInvoiceMongoLineCommandHandler(IMongoInvoiceRepository mongoInvoiceRepository)
        {
            _mongoInvoiceRepository = mongoInvoiceRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(DeleteMongoInvoiceMongoLineCommand request, CancellationToken cancellationToken)
        {
            var aggregateRoot = await _mongoInvoiceRepository.FindByIdAsync(request.MongoInvoiceId, cancellationToken);
            if (aggregateRoot is null)
            {
                throw new NotFoundException($"{nameof(MongoInvoice)} of Id '{request.MongoInvoiceId}' could not be found");
            }

            var existingMongoLine = aggregateRoot.MongoLines.FirstOrDefault(p => p.Id == request.Id);
            if (existingMongoLine is null)
            {
                throw new NotFoundException($"{nameof(MongoLine)} of Id '{request.Id}' could not be found associated with {nameof(MongoInvoice)} of Id '{request.MongoInvoiceId}'");
            }

            aggregateRoot.MongoLines.Remove(existingMongoLine);

            _mongoInvoiceRepository.Update(aggregateRoot);
        }
    }

    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeleteMongoInvoiceMongoLineCommandValidator : AbstractValidator<DeleteMongoInvoiceMongoLineCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteMongoInvoiceMongoLineCommandValidator()
        {
            // IntentFully(Match = "ConfigureValidationRules")
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.MongoInvoiceId)
                .NotNull();

            RuleFor(v => v.Id)
                .NotNull();
        }
    }
}