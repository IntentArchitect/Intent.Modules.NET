using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Domain.Entities.UniqueIndexConstraint;
using CleanArchitecture.Comprehensive.Domain.Repositories.UniqueIndexConstraint;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.UniqueIndexConstraint.ClassicMapping.CreateAggregateWithUniqueConstraintIndexStereotype
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateAggregateWithUniqueConstraintIndexStereotypeCommandHandler : IRequestHandler<CreateAggregateWithUniqueConstraintIndexStereotypeCommand, Guid>
    {
        private readonly IAggregateWithUniqueConstraintIndexStereotypeRepository _aggregateWithUniqueConstraintIndexStereotypeRepository;

        [IntentManaged(Mode.Merge)]
        public CreateAggregateWithUniqueConstraintIndexStereotypeCommandHandler(IAggregateWithUniqueConstraintIndexStereotypeRepository aggregateWithUniqueConstraintIndexStereotypeRepository)
        {
            _aggregateWithUniqueConstraintIndexStereotypeRepository = aggregateWithUniqueConstraintIndexStereotypeRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(
            CreateAggregateWithUniqueConstraintIndexStereotypeCommand request,
            CancellationToken cancellationToken)
        {
            var newAggregateWithUniqueConstraintIndexStereotype = new AggregateWithUniqueConstraintIndexStereotype
            {
                SingleUniqueField = request.SingleUniqueField,
                CompUniqueFieldA = request.CompUniqueFieldA,
                CompUniqueFieldB = request.CompUniqueFieldB,
            };

            _aggregateWithUniqueConstraintIndexStereotypeRepository.Add(newAggregateWithUniqueConstraintIndexStereotype);
            await _aggregateWithUniqueConstraintIndexStereotypeRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return newAggregateWithUniqueConstraintIndexStereotype.Id;
        }
    }
}