using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CqrsAutoCrud.TestApplication.Domain.Common;
using CqrsAutoCrud.TestApplication.Domain.Entities;
using CqrsAutoCrud.TestApplication.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.AggregateRootCompositeMany.UpdateAggregateRootCompositeMany
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateAggregateRootCompositeManyCommandHandler : IRequestHandler<UpdateAggregateRootCompositeManyCommand>
    {
        private readonly IAggregateRootRepository _aggregateRootRepository;

        [IntentManaged(Mode.Ignore)]
        public UpdateAggregateRootCompositeManyCommandHandler(IAggregateRootRepository aggregateRootRepository)
        {
            _aggregateRootRepository = aggregateRootRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Unit> Handle(UpdateAggregateRootCompositeManyCommand request, CancellationToken cancellationToken)
        {
            var existingAggregateRoot = await _aggregateRootRepository.FindByIdAsync(request.AggregateRootId, cancellationToken);
            if (existingAggregateRoot == null)
            {
                throw new InvalidOperationException($"{nameof(AggregateRoot)} of Id '{request.AggregateRootId}' could not be found");
            }

            var element = existingAggregateRoot.Composites.FirstOrDefault(p => p.Id == request.Id);
            if (element == null)
            {
                throw new InvalidOperationException($"{nameof(CompositeManyB)} of Id '{request.Id}' could not be found associated with {nameof(AggregateRoot)} of Id '{request.AggregateRootId}'");
            }

            element.UpdateObject(request, UpdateCompositesCompositeManyB);
            
            return Unit.Value;
        }

       

        [IntentManaged(Mode.Fully)]
        private static void UpdateCompositesCompositeManyB(CompositeManyB entity, UpdateAggregateRootCompositeManyCommand dto)
        {
            entity.CompositeAttr = dto.CompositeAttr;
            entity.AAggregaterootId = dto.AggregateRootId;
            entity.Composite = dto.Composite != null
                ? (entity.Composite ?? new CompositeSingleBB()).UpdateObject(dto.Composite, UpdateCompositeCompositeSingleBB)
                : null;
            entity.Composites.UpdateCollection(dto.Composites, (x, y) => x.Id == y.Id, UpdateCompositesCompositeManyBB);
        }

        [IntentManaged(Mode.Fully)]
        private static void UpdateCompositeCompositeSingleBB(CompositeSingleBB entity, UpdateCompositeSingleBBDTO dto)
        {
            entity.CompositeAttr = dto.CompositeAttr;
        }

        [IntentManaged(Mode.Fully)]
        private static void UpdateCompositesCompositeManyBB(CompositeManyBB entity, UpdateCompositeManyBBDTO dto)
        {
            entity.CompositeAttr = dto.CompositeAttr;
            entity.ACompositeManyId = dto.ACompositeManyId;
        }
    }

}