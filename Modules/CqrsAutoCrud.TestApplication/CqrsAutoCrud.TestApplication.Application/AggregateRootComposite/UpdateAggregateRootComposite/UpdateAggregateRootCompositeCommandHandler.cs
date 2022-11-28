using System;
using System.Threading;
using System.Threading.Tasks;
using CqrsAutoCrud.TestApplication.Domain.Common;
using CqrsAutoCrud.TestApplication.Domain.Entities;
using CqrsAutoCrud.TestApplication.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.AggregateRootComposite.UpdateAggregateRootComposite
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateAggregateRootCompositeCommandHandler : IRequestHandler<UpdateAggregateRootCompositeCommand>
    {
        private readonly IAggregateRootRepository _aggregateRootRepository;

        [IntentManaged(Mode.Ignore)]
        public UpdateAggregateRootCompositeCommandHandler(IAggregateRootRepository aggregateRootRepository)
        {
            _aggregateRootRepository = aggregateRootRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Unit> Handle(UpdateAggregateRootCompositeCommand request, CancellationToken cancellationToken)
        {
            var existingAggregateRoot = await _aggregateRootRepository.FindByIdAsync(request.AggregateRootId, cancellationToken);
            if (existingAggregateRoot == null)
            {
                throw new InvalidOperationException($"{nameof(AggregateRoot)} of Id '{request.AggregateRootId}' could not be found");
            }

            existingAggregateRoot.Composite = request.Composite != null
                 ? (existingAggregateRoot.Composite ?? new CompositeSingleA()).UpdateObject(request, UpdateCompositeCompositeSingleA)
                 : null;
            
            return Unit.Value;
        }

        [IntentManaged(Mode.Fully)]
        private static void UpdateCompositeCompositeSingleA(CompositeSingleA entity, UpdateAggregateRootCompositeCommand dto)
        {
            entity.CompositeAttr = dto.CompositeAttr;
            entity.Composite = dto.Composite != null
                ? (entity.Composite ?? new CompositeSingleAA()).UpdateObject(dto.Composite, UpdateCompositeCompositeSingleAA)
                : null;
            entity.Composites.UpdateCollection(dto.Composites, (x, y) => x.Id == y.Id, UpdateCompositesCompositeManyAA);
        }

        [IntentManaged(Mode.Fully)]
        private static void UpdateCompositeCompositeSingleAA(CompositeSingleAA entity, UpdateCompositeSingleAADTO dto)
        {
            entity.CompositeAttr = dto.CompositeAttr;
        }

        [IntentManaged(Mode.Fully)]
        private static void UpdateCompositesCompositeManyAA(CompositeManyAA entity, UpdateCompositeManyAADTO dto)
        {
            entity.CompositeAttr = dto.CompositeAttr;
            entity.ACompositeSingleId = dto.ACompositeSingleId;
        }
    }

}