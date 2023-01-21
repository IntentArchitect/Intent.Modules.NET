using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CqrsAutoCrud.TestApplication.Domain.Common;
using CqrsAutoCrud.TestApplication.Domain.Entities;
using CqrsAutoCrud.TestApplication.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.AggregateRoots.UpdateAggregateRoot
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
            existingAggregateRoot.Composite = request.Composite != null
                ? (existingAggregateRoot.Composite ?? new CompositeSingleA()).UpdateObject(request.Composite, UpdateCompositeSingleA)
                : null;
            existingAggregateRoot.Composites.UpdateCollection(request.Composites, (e, d) => e.Id == d.Id, UpdateCompositeManyB);
            return Unit.Value;
        }

        [IntentManaged(Mode.Fully)]
        private static void UpdateCompositeSingleA(CompositeSingleA entity, UpdateAggregateRootCompositeSingleADTO dto)
        {
            entity.CompositeAttr = dto.CompositeAttr;
            entity.Composite = dto.Composite != null
                ? (entity.Composite ?? new CompositeSingleAA()).UpdateObject(dto.Composite, UpdateCompositeSingleAA)
                : null;
            entity.Composites.UpdateCollection(dto.Composites, (e, d) => e.Id == d.Id, UpdateCompositeManyAA);
        }

        [IntentManaged(Mode.Fully)]
        private static void UpdateCompositeSingleAA(CompositeSingleAA entity, UpdateAggregateRootCompositeSingleACompositeSingleAADTO dto)
        {
            entity.CompositeAttr = dto.CompositeAttr;
        }

        [IntentManaged(Mode.Fully)]
        private static void UpdateCompositeManyAA(CompositeManyAA entity, UpdateAggregateRootCompositeSingleACompositeManyAADTO dto)
        {
            entity.CompositeAttr = dto.CompositeAttr;
            entity.ACompositeSingleId = dto.ACompositeSingleId;
        }

        [IntentManaged(Mode.Fully)]
        private static void UpdateCompositeManyB(CompositeManyB entity, UpdateAggregateRootCompositeManyBDTO dto)
        {
            entity.CompositeAttr = dto.CompositeAttr;
            entity.AggregateRootId = dto.AggregateRootId;
            entity.SomeDate = dto.SomeDate;
            entity.Composite = dto.Composite != null
                ? (entity.Composite ?? new CompositeSingleBB()).UpdateObject(dto.Composite, UpdateCompositeSingleBB)
                : null;
            entity.Composites.UpdateCollection(dto.Composites, (e, d) => e.Id == d.Id, UpdateCompositeManyBB);
        }

        [IntentManaged(Mode.Fully)]
        private static void UpdateCompositeSingleBB(CompositeSingleBB entity, UpdateAggregateRootCompositeManyBCompositeSingleBBDTO dto)
        {
            entity.CompositeAttr = dto.CompositeAttr;
        }

        [IntentManaged(Mode.Fully)]
        private static void UpdateCompositeManyBB(CompositeManyBB entity, UpdateAggregateRootCompositeManyBCompositeManyBBDTO dto)
        {
            entity.CompositeAttr = dto.CompositeAttr;
            entity.ACompositeManyId = dto.ACompositeManyId;
        }
    }
}