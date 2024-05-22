using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using ValueObjects.Record.Domain;
using ValueObjects.Record.Domain.Common.Exceptions;
using ValueObjects.Record.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace ValueObjects.Record.Application.TestEntities.UpdateTestEntity
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateTestEntityCommandHandler : IRequestHandler<UpdateTestEntityCommand>
    {
        private readonly ITestEntityRepository _testEntityRepository;

        [IntentManaged(Mode.Merge)]
        public UpdateTestEntityCommandHandler(ITestEntityRepository testEntityRepository)
        {
            _testEntityRepository = testEntityRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateTestEntityCommand request, CancellationToken cancellationToken)
        {
            var testEntity = await _testEntityRepository.FindByIdAsync(request.Id, cancellationToken);
            if (testEntity is null)
            {
                throw new NotFoundException($"Could not find TestEntity '{request.Id}'");
            }

            testEntity.Name = request.Name;
            testEntity.Amount = new Money(
                Amount: request.Amount.Amount,
                Currency: request.Amount.Currency);
            testEntity.Address = new Address(
                Line1: request.Address.Line1,
                Line2: request.Address.Line2,
                City: request.Address.City,
                Country: request.Address.Country,
                AddressType: request.Address.AddressType);
        }
    }
}