using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using ValueObjects.Class.Domain;
using ValueObjects.Class.Domain.Common.Exceptions;
using ValueObjects.Class.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace ValueObjects.Class.Application.TestEntities.UpdateTestEntity
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
                amount: request.Amount.Amount,
                currency: request.Amount.Currency);
            testEntity.Address = new Address(
                line1: request.Address.Line1,
                line2: request.Address.Line2,
                city: request.Address.City,
                country: request.Address.Country,
                addressType: request.Address.AddressType);
        }
    }
}