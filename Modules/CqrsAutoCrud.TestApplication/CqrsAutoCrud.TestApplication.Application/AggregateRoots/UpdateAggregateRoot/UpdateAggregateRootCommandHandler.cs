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

namespace CqrsAutoCrud.TestApplication.Application.AggregateRoots.UpdateAggregateRoot
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateAggregateRootCommandHandler : IRequestHandler<UpdateAggregateRootCommand>
    {
        private IAggregateRootRepository _aggregateRootRepository;

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
                ? new CompositeSingleA
                {
                    Id = request.Composite.Id,
                    CompositeAttr = request.Composite.CompositeAttr,
                    Composite = request.Composite.Composite != null
                        ? new CompositeSingleAA
                        {
                            Id = request.Composite.Composite.Id,
                            CompositeAttr = request.Composite.Composite.CompositeAttr,
                        }
                        : null,
                    Composites = request.Composite.Composites.Select(composites =>
                        new CompositeManyAA
                        {
                            Id = composites.Id,
                            CompositeAttr = composites.CompositeAttr,
                            ACompositeSingleId = composites.ACompositeSingleId,
                        }).ToList(),
                }
                : null;
            existingAggregateRoot.Composites = request.Composites.Select(composites =>
                new CompositeManyB
                {
                    Id = composites.Id,
                    CompositeAttr = composites.CompositeAttr,
                    AAggregaterootId = composites.AAggregaterootId,
                    Composite = composites.Composite != null
                        ? new CompositeSingleBB
                        {
                            Id = composites.Composite.Id,
                            CompositeAttr = composites.Composite.CompositeAttr,
                        }
                        : null,
                    Composites = composites.Composites.Select(composites =>
                        new CompositeManyBB
                        {
                            Id = composites.Id,
                            CompositeAttr = composites.CompositeAttr,
                            ACompositeManyId = composites.ACompositeManyId,
                        }).ToList(),
                }).ToList();
#warning Property not a composite association: Aggregate

            return Unit.Value;
        }
    }
}