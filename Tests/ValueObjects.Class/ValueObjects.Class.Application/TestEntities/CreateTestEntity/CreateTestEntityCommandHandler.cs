using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using ValueObjects.Class.Domain;
using ValueObjects.Class.Domain.Entities;
using ValueObjects.Class.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace ValueObjects.Class.Application.TestEntities.CreateTestEntity
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
                    amount: request.Amount.Amount,
                    currency: request.Amount.Currency),
                Address = new Address(
                    line1: request.Address.Line1,
                    line2: request.Address.Line2,
                    city: request.Address.City,
                    country: request.Address.Country,
                    addressType: request.Address.AddressType)
            };

            _testEntityRepository.Add(testEntity);
            await _testEntityRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return testEntity.Id;
        }
    }
}