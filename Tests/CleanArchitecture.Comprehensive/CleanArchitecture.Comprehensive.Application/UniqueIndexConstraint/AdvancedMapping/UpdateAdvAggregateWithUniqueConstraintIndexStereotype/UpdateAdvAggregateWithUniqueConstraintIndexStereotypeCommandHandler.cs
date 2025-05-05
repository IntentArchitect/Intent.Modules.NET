using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Domain.Common;
using CleanArchitecture.Comprehensive.Domain.Common.Exceptions;
using CleanArchitecture.Comprehensive.Domain.Entities.UniqueIndexConstraint;
using CleanArchitecture.Comprehensive.Domain.Repositories.UniqueIndexConstraint;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.UniqueIndexConstraint.AdvancedMapping.UpdateAdvAggregateWithUniqueConstraintIndexStereotype
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateAdvAggregateWithUniqueConstraintIndexStereotypeCommandHandler : IRequestHandler<UpdateAdvAggregateWithUniqueConstraintIndexStereotypeCommand>
    {
        private readonly IAggregateWithUniqueConstraintIndexStereotypeRepository _aggregateWithUniqueConstraintIndexStereotypeRepository;

        [IntentManaged(Mode.Merge)]
        public UpdateAdvAggregateWithUniqueConstraintIndexStereotypeCommandHandler(IAggregateWithUniqueConstraintIndexStereotypeRepository aggregateWithUniqueConstraintIndexStereotypeRepository)
        {
            _aggregateWithUniqueConstraintIndexStereotypeRepository = aggregateWithUniqueConstraintIndexStereotypeRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(
            UpdateAdvAggregateWithUniqueConstraintIndexStereotypeCommand request,
            CancellationToken cancellationToken)
        {
            var aggregateWithUniqueConstraintIndexStereotype = await _aggregateWithUniqueConstraintIndexStereotypeRepository.FindByIdAsync(request.Id, cancellationToken);
            if (aggregateWithUniqueConstraintIndexStereotype is null)
            {
                throw new NotFoundException($"Could not find AggregateWithUniqueConstraintIndexStereotype '{request.Id}'");
            }

            aggregateWithUniqueConstraintIndexStereotype.SingleUniqueField = request.SingleUniqueField;
            aggregateWithUniqueConstraintIndexStereotype.CompUniqueFieldA = request.CompUniqueFieldA;
            aggregateWithUniqueConstraintIndexStereotype.CompUniqueFieldB = request.CompUniqueFieldB;
            aggregateWithUniqueConstraintIndexStereotype.UniqueConstraintIndexCompositeEntityForStereotypes = UpdateHelper.CreateOrUpdateCollection(aggregateWithUniqueConstraintIndexStereotype.UniqueConstraintIndexCompositeEntityForStereotypes, request.UniqueConstraintIndexCompositeEntityForStereotypes, (e, d) => e.Id == d.Id, CreateOrUpdateUniqueConstraintIndexCompositeEntityForStereotype);
        }

        [IntentManaged(Mode.Fully)]
        private static UniqueConstraintIndexCompositeEntityForStereotype CreateOrUpdateUniqueConstraintIndexCompositeEntityForStereotype(
            UniqueConstraintIndexCompositeEntityForStereotype? entity,
            UpdateAdvAggregateWithUniqueConstraintIndexStereotypeCommandUniqueConstraintIndexCompositeEntityForStereotypesDto dto)
        {
            entity ??= new UniqueConstraintIndexCompositeEntityForStereotype();
            entity.Id = dto.Id;
            entity.Field = dto.Field;
            return entity;
        }
    }
}