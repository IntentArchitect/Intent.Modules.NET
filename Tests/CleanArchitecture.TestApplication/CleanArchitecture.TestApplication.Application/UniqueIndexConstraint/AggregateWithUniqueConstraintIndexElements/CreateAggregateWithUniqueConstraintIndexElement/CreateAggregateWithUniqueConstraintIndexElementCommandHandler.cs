using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.TestApplication.Domain.Entities.UniqueIndexConstraint;
using CleanArchitecture.TestApplication.Domain.Repositories.UniqueIndexConstraint;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.TestApplication.Application.UniqueIndexConstraint.AggregateWithUniqueConstraintIndexElements.CreateAggregateWithUniqueConstraintIndexElement
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateAggregateWithUniqueConstraintIndexElementCommandHandler : IRequestHandler<CreateAggregateWithUniqueConstraintIndexElementCommand, Guid>
    {
        private readonly IAggregateWithUniqueConstraintIndexElementRepository _aggregateWithUniqueConstraintIndexElementRepository;

        [IntentManaged(Mode.Merge)]
        public CreateAggregateWithUniqueConstraintIndexElementCommandHandler(IAggregateWithUniqueConstraintIndexElementRepository aggregateWithUniqueConstraintIndexElementRepository)
        {
            _aggregateWithUniqueConstraintIndexElementRepository = aggregateWithUniqueConstraintIndexElementRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(
            CreateAggregateWithUniqueConstraintIndexElementCommand request,
            CancellationToken cancellationToken)
        {
            var newAggregateWithUniqueConstraintIndexElement = new AggregateWithUniqueConstraintIndexElement
            {
                SingleUniqueField = request.SingleUniqueField,
                CompUniqueFieldA = request.CompUniqueFieldA,
                CompUniqueFieldB = request.CompUniqueFieldB,
            };

            _aggregateWithUniqueConstraintIndexElementRepository.Add(newAggregateWithUniqueConstraintIndexElement);
            await _aggregateWithUniqueConstraintIndexElementRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return newAggregateWithUniqueConstraintIndexElement.Id;
        }
    }
}