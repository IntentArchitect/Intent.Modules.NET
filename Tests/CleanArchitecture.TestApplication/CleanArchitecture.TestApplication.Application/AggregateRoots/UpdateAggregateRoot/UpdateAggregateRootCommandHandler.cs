using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.TestApplication.Domain.Common;
using CleanArchitecture.TestApplication.Domain.Entities;
using CleanArchitecture.TestApplication.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.AggregateRoots.UpdateAggregateRoot
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateAggregateRootCommandHandler : IRequestHandler<UpdateAggregateRootCommand>
    {
        private readonly IAggregateRootRepository _aggregateRootRepository;

        [IntentManaged(Mode.Ignore)]
        public UpdateAggregateRootCommandHandler(IAggregateRootRepository aggregateRootRepository)
        {
            _aggregateRootRepository = aggregateRootRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Unit> Handle(UpdateAggregateRootCommand request, CancellationToken cancellationToken)
        {
            var existingAggregateRoot = await _aggregateRootRepository.FindByIdAsync(request.Id, cancellationToken);
            existingAggregateRoot.AggregateAttr = request.AggregateAttr;
            existingAggregateRoot.Composites.UpdateCollection(request.Composites, (e, d) => e.Id == d.Id, UpdateCompositeManyB);
            existingAggregateRoot.Composite = request.Composite != null
                ? (existingAggregateRoot.Composite ?? new CompositeSingleA()).UpdateObject(request.Composite, UpdateCompositeSingleA)
                : null;
#warning Field not a composite association: Aggregate
            return Unit.Value;
        }

        [IntentManaged(Mode.Fully)]
        private static void UpdateCompositeManyB(CompositeManyB entity, UpdateAggregateRootCompositeManyBDto dto)
        {
            entity.CompositeAttr = dto.CompositeAttr;
            entity.SomeDate = dto.SomeDate;
            entity.AggregateRootId = dto.AggregateRootId;
            entity.Composites.UpdateCollection(dto.Composites, (e, d) => e.Id == d.Id, UpdateCompositeManyBB);
            entity.Composite = dto.Composite != null
                ? (entity.Composite ?? new CompositeSingleBB()).UpdateObject(dto.Composite, UpdateCompositeSingleBB)
                : null;
        }

        [IntentManaged(Mode.Fully)]
        private static void UpdateCompositeSingleBB(CompositeSingleBB entity, UpdateAggregateRootCompositeManyBCompositeSingleBBDto dto)
        {
            entity.CompositeAttr = dto.CompositeAttr;
        }

        [IntentManaged(Mode.Fully)]
        private static void UpdateCompositeManyBB(CompositeManyBB entity, UpdateAggregateRootCompositeManyBCompositeManyBBDto dto)
        {
            entity.CompositeAttr = dto.CompositeAttr;
            entity.CompositeManyBId = dto.CompositeManyBId;
        }

        [IntentManaged(Mode.Fully)]
        private static void UpdateCompositeSingleA(CompositeSingleA entity, UpdateAggregateRootCompositeSingleADto dto)
        {
            entity.CompositeAttr = dto.CompositeAttr;
            entity.Composite = dto.Composite != null
                ? (entity.Composite ?? new CompositeSingleAA()).UpdateObject(dto.Composite, UpdateCompositeSingleAA)
                : null;
            entity.Composites.UpdateCollection(dto.Composites, (e, d) => e.Id == d.Id, UpdateCompositeManyAA);
        }

        [IntentManaged(Mode.Fully)]
        private static void UpdateCompositeSingleAA(CompositeSingleAA entity, UpdateAggregateRootCompositeSingleACompositeSingleAADto dto)
        {
            entity.CompositeAttr = dto.CompositeAttr;
        }

        [IntentManaged(Mode.Fully)]
        private static void UpdateCompositeManyAA(CompositeManyAA entity, UpdateAggregateRootCompositeSingleACompositeManyAADto dto)
        {
            entity.CompositeAttr = dto.CompositeAttr;
            entity.CompositeSingleAId = dto.CompositeSingleAId;
        }
    }
}