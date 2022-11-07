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

namespace CqrsAutoCrud.TestApplication.Application.AggregateRoots.CreateAggregateRoot
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateAggregateRootCommandHandler : IRequestHandler<CreateAggregateRootCommand, Guid>
    {
        private IAggregateRootRepository _aggregateRootRepository;

        [IntentManaged(Mode.Ignore)]
        public CreateAggregateRootCommandHandler(IAggregateRootRepository aggregateRootRepository)
        {
            _aggregateRootRepository = aggregateRootRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateAggregateRootCommand request, CancellationToken cancellationToken)
        {
            var newAggregateRoot = new AggregateRoot
            {
                AggregateAttr = request.AggregateAttr,
                Composite = request.Composite != null
                    ? new CompositeSingleA
                    {
                        Id = IdentityGenerator.NewSequentialId(),
                        CompositeAttr = request.Composite.CompositeAttr,
                        Composite = request.Composite.Composite != null
                            ? new CompositeSingleAA
                            {
                                Id = IdentityGenerator.NewSequentialId(),
                                CompositeAttr = request.Composite.Composite.CompositeAttr,
                            }
                            : null,
                        Composites = request.Composite.Composites?.Select(composites =>
                            new CompositeManyAA
                            {
                                Id = IdentityGenerator.NewSequentialId(),
                                CompositeAttr = composites.CompositeAttr,
                                ACompositeSingleId = composites.ACompositeSingleId,
                            }).ToList(),
                    }
                    : null,
                Composites = request.Composites?.Select(composites =>
                    new CompositeManyB
                    {
                        Id = IdentityGenerator.NewSequentialId(),
                        CompositeAttr = composites.CompositeAttr,
                        AAggregaterootId = composites.AAggregaterootId,
                        Composite = composites.Composite != null
                            ? new CompositeSingleBB
                            {
                                Id = IdentityGenerator.NewSequentialId(),
                                CompositeAttr = composites.Composite.CompositeAttr,
                            }
                            : null,
                        Composites = composites.Composites?.Select(composites =>
                            new CompositeManyBB
                            {
                                Id = IdentityGenerator.NewSequentialId(),
                                CompositeAttr = composites.CompositeAttr,
                                ACompositeManyId = composites.ACompositeManyId,
                            }).ToList(),
                    }).ToList(),
#warning Property not a composite association: Aggregate
            };

            _aggregateRootRepository.Add(newAggregateRoot);

            await _aggregateRootRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return newAggregateRoot.Id;
        }
    }
}