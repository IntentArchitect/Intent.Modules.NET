using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CqrsAutoCrud.TestApplication.Domain.Entities;
using CqrsAutoCrud.TestApplication.Domain.Entities.Common;
using CqrsAutoCrud.TestApplication.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.AggregateRootAS.CreateAggregateRootA
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateAggregateRootACommandHandler : IRequestHandler<CreateAggregateRootACommand, Guid>
    {
        private IAggregateRootARepository _aggregateRootARepository;

        [IntentManaged(Mode.Ignore)]
        public CreateAggregateRootACommandHandler(IAggregateRootARepository aggregateRootARepository)
        {
            _aggregateRootARepository = aggregateRootARepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateAggregateRootACommand request, CancellationToken cancellationToken)
        {
            var newAggregateRootA = new AggregateRootA
            {
                Id = IdentityGenerator.NewSequentialId(),
                AggregateAttr = request.AggregateAttr,
                Composite = request.Composite != null
                    ? new CompositeSingleAA
                    {
                        Id = IdentityGenerator.NewSequentialId(),
                        CompositeAttr = request.Composite.CompositeAttr,
                        Composite = request.Composite.Composite != null
                            ? new CompositeSingleAAA1
                            {
                                Id = IdentityGenerator.NewSequentialId(),
                                CompositeAttr = request.Composite.Composite.CompositeAttr,
                            }
                            : null,
                        Composites = request.Composite.Composites?.Select(composites =>
                            new CompositeManyAAA1
                            {
                                Id = IdentityGenerator.NewSequentialId(),
                                CompositeAttr = composites.CompositeAttr,
                                ACompositeSingleId = composites.ACompositeSingleId,
                            }).ToList(),
                    }
                    : null,
                Composites = request.Composites?.Select(composites =>
                    new CompositeManyAA
                    {
                        Id = IdentityGenerator.NewSequentialId(),
                        CompositeAttr = composites.CompositeAttr,
                        AAggregaterootId = composites.AAggregaterootId,
                        Composite = composites.Composite != null
                            ? new CompositeSingleAAA2
                            {
                                Id = IdentityGenerator.NewSequentialId(),
                                CompositeAttr = composites.Composite.CompositeAttr,
                            }
                            : null,
                        Composites = composites.Composites?.Select(composites =>
                            new CompositeManyAAA2
                            {
                                Id = IdentityGenerator.NewSequentialId(),
                                CompositeAttr = composites.CompositeAttr,
                                ACompositeManyId = composites.ACompositeManyId,
                            }).ToList(),
                    }).ToList(),
                Aggregate = request.Aggregate != null
                    ? new AggregateSingleAA
                    {
                        Id = IdentityGenerator.NewSequentialId(),
                        AggregationAttr = request.Aggregate.AggregationAttr,
                    }
                    : null,
            };

            _aggregateRootARepository.Add(newAggregateRootA);

            await _aggregateRootARepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return newAggregateRootA.Id;
        }
    }
}