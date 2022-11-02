using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CqrsAutoCrud.TestApplication.Domain.Entities;
using CqrsAutoCrud.TestApplication.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.AggregateRootAS.UpdateAggregateRootA
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateAggregateRootACommandHandler : IRequestHandler<UpdateAggregateRootACommand>
    {
        private IAggregateRootARepository _aggregateRootARepository;

        [IntentManaged(Mode.Ignore)]
        public UpdateAggregateRootACommandHandler(IAggregateRootARepository aggregateRootARepository)
        {
            _aggregateRootARepository = aggregateRootARepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Unit> Handle(UpdateAggregateRootACommand request, CancellationToken cancellationToken)
        {
            var existingAggregateRootA = await _aggregateRootARepository.FindByIdAsync(request.Id, cancellationToken);
            existingAggregateRootA.AggregateAttr = request.AggregateAttr;
            existingAggregateRootA.Composite = request.Composite != null
                ? new CompositeSingleAA
                {
                    Id = request.Composite.Id,
                    CompositeAttr = request.Composite.CompositeAttr,
                    Composite = request.Composite.Composite != null
                        ? new CompositeSingleAAA1
                        {
                            Id = request.Composite.Composite.Id,
                            CompositeAttr = request.Composite.Composite.CompositeAttr,
                        }
                        : null,
                    Composites = request.Composite.Composites?.Select(composites =>
                        new CompositeManyAAA1
                        {
                            Id = composites.Id,
                            CompositeAttr = composites.CompositeAttr,
                            ACompositeSingleId = composites.ACompositeSingleId,
                        }).ToList(),
                }
                : null;
            existingAggregateRootA.Composites = request.Composites?.Select(composites =>
                new CompositeManyAA
                {
                    Id = composites.Id,
                    CompositeAttr = composites.CompositeAttr,
                    AAggregaterootId = composites.AAggregaterootId,
                    Composite = composites.Composite != null
                        ? new CompositeSingleAAA2
                        {
                            Id = composites.Composite.Id,
                            CompositeAttr = composites.Composite.CompositeAttr,
                        }
                        : null,
                    Composites = composites.Composites?.Select(composites =>
                        new CompositeManyAAA2
                        {
                            Id = composites.Id,
                            CompositeAttr = composites.CompositeAttr,
                            ACompositeManyId = composites.ACompositeManyId,
                        }).ToList(),
                }).ToList();
            existingAggregateRootA.Aggregate = request.Aggregate != null
                ? new AggregateSingleAA
                {
                    Id = request.Aggregate.Id,
                    AggregationAttr = request.Aggregate.AggregationAttr,
                }
                : null;

            return Unit.Value;
        }
    }
}