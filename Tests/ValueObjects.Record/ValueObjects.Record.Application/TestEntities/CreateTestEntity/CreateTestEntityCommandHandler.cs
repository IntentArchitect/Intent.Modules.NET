using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using ValueObjects.Record.Domain;
using ValueObjects.Record.Domain.Entities;
using ValueObjects.Record.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace ValueObjects.Record.Application.TestEntities.CreateTestEntity
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateTestEntityCommandHandler : IRequestHandler<CreateTestEntityCommand, Guid>
    {
        private readonly ITestEntityRepository _testEntityRepository;

        [IntentManaged(Mode.Merge)]
        public CreateTestEntityCommandHandler(ITestEntityRepository testEntityRepository)
        {
            _testEntityRepository = testEntityRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateTestEntityCommand request, CancellationToken cancellationToken)
        {
            var testEntity = new TestEntity
            {
                Name = request.Name,
                Amount = new Money(
                    Amount: request.Amount.Amount,
                    Currency: request.Amount.Currency),
                Address = new Address(
                    Line1: request.Address.Line1,
                    Line2: request.Address.Line2,
                    City: request.Address.City,
                    Country: request.Address.Country,
                    AddressType: request.Address.AddressType)
            };

            _testEntityRepository.Add(testEntity);
            await _testEntityRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return testEntity.Id;
        }
    }
}