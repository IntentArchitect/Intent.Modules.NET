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

namespace CleanArchitecture.TestApplication.Application.AggregateRoots.UpdateAggregateRootCompositeManyB
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateAggregateRootCompositeManyBCommandHandler : IRequestHandler<UpdateAggregateRootCompositeManyBCommand>
    {
        private readonly IAggregateRootRepository _aggregateRootRepository;

        [IntentManaged(Mode.Ignore)]
        public UpdateAggregateRootCompositeManyBCommandHandler(IAggregateRootRepository aggregateRootRepository)
        {
            _aggregateRootRepository = aggregateRootRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Unit> Handle(UpdateAggregateRootCompositeManyBCommand request, CancellationToken cancellationToken)
        {
            var aggregateRoot = await _aggregateRootRepository.FindByIdAsync(request.AggregateRootId, cancellationToken);
            if (aggregateRoot == null)
            {
                throw new InvalidOperationException($"{nameof(AggregateRoot)} of Id '{request.AggregateRootId}' could not be found");
            }
            var element = aggregateRoot.Composites.FirstOrDefault(p => p.Id == request.Id);
            if (element == null)
            {
                throw new InvalidOperationException($"{nameof(CompositeManyB)} of Id '{request.Id}' could not be found associated with {nameof(AggregateRoot)} of Id '{request.AggregateRootId}'");
            }
            element.AggregateRootId = request.AggregateRootId;
            element.CompositeAttr = request.CompositeAttr;
            element.SomeDate = request.SomeDate;
            element.Composite = request.Composite != null
                ? (element.Composite ?? new CompositeSingleBB()).UpdateObject(request.Composite, UpdateCompositeSingleBB)
                : null;
            element.Composites.UpdateCollection(request.Composites, (e, d) => e.Id == d.Id, UpdateCompositeManyBB);
            return Unit.Value;
        }

        [IntentManaged(Mode.Fully)]
        private static void UpdateCompositeSingleBB(CompositeSingleBB entity, UpdateAggregateRootCompositeManyBCompositeSingleBBDto dto)
        {
            entity.CompositeAttr = dto.CompositeAttr;
            entity.CompositeAttr = dto.CompositeAttr;
        }

        [IntentManaged(Mode.Fully)]
        private static void UpdateCompositeManyBB(CompositeManyBB entity, UpdateAggregateRootCompositeManyBCompositeManyBBDto dto)
        {
            entity.CompositeAttr = dto.CompositeAttr;
            entity.CompositeManyBId = dto.CompositeManyBId;
            entity.CompositeAttr = dto.CompositeAttr;
            entity.CompositeManyBId = dto.CompositeManyBId;
        }
    }
}