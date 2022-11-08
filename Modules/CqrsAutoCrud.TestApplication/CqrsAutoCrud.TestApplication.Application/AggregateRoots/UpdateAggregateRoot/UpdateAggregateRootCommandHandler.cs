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
                ? (existingAggregateRoot.Composite ?? new CompositeSingleA()).UpdateObject(
                    request.Composite,
                    (existingEntity1, request1) =>
                    {
                        existingEntity1.CompositeAttr = request1.CompositeAttr;
                        existingEntity1.Composite = request1.Composite != null
                            ? (existingEntity1.Composite ?? new CompositeSingleAA()).UpdateObject(
                                request1.Composite,
                                (existingEntity2, request2) =>
                                {
                                    existingEntity2.CompositeAttr = request2.CompositeAttr;
                                }
                            )
                            : null;

                        existingEntity1.Composites.UpdateCollection(request1.Composites, (x, y) => x.Id == y.Id, (existingEntity3, request3) =>
                        {
                            existingEntity3.CompositeAttr = request3.CompositeAttr;
                        });
                    })
                : null;
            existingAggregateRoot.Composites.UpdateCollection(request.Composites, (x, y) => x.Id == y.Id, (existingEntity4, request4) =>
            {
                existingEntity4.CompositeAttr = request4.CompositeAttr;
                existingEntity4.AAggregaterootId = request4.AAggregaterootId;
                existingEntity4.Composite = request4.Composite != null
                    ? (existingEntity4.Composite ?? new CompositeSingleBB()).UpdateObject(
                        request4.Composite,
                        (existingEntity5, request5) =>
                        {
                            existingEntity5.CompositeAttr = request5.CompositeAttr;
                        })
                    : null;
                existingEntity4.Composites.UpdateCollection(request4.Composites, (x, y) => x.Id == y.Id, (existingEntity6, request6) =>
                {
                    existingEntity6.CompositeAttr = request6.CompositeAttr;
                });
            });
            
#warning Property not a composite association: Aggregate
            
            return Unit.Value;
        }
    }
}