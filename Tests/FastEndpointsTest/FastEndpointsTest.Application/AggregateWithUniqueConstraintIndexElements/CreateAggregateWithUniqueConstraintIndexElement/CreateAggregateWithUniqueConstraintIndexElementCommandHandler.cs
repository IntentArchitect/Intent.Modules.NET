using System;
using System.Threading;
using System.Threading.Tasks;
using FastEndpointsTest.Domain.Entities.UniqueIndexConstraint;
using FastEndpointsTest.Domain.Repositories.UniqueIndexConstraint;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace FastEndpointsTest.Application.AggregateWithUniqueConstraintIndexElements.CreateAggregateWithUniqueConstraintIndexElement
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
            var aggregateWithUniqueConstraintIndexElement = new AggregateWithUniqueConstraintIndexElement(
                singleUniqueField: request.SingleUniqueField,
                compUniqueFieldA: request.CompUniqueFieldA,
                compUniqueFieldB: request.CompUniqueFieldB);

            _aggregateWithUniqueConstraintIndexElementRepository.Add(aggregateWithUniqueConstraintIndexElement);
            await _aggregateWithUniqueConstraintIndexElementRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return aggregateWithUniqueConstraintIndexElement.Id;
        }
    }
}