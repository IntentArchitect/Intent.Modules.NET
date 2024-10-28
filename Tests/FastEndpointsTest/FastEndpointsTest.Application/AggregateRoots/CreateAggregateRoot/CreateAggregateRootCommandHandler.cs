using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FastEndpointsTest.Domain.Entities.CRUD;
using FastEndpointsTest.Domain.Repositories.CRUD;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace FastEndpointsTest.Application.AggregateRoots.CreateAggregateRoot
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateAggregateRootCommandHandler : IRequestHandler<CreateAggregateRootCommand, Guid>
    {
        private readonly IAggregateRootRepository _aggregateRootRepository;

        [IntentManaged(Mode.Merge)]
        public CreateAggregateRootCommandHandler(IAggregateRootRepository aggregateRootRepository)
        {
            _aggregateRootRepository = aggregateRootRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateAggregateRootCommand request, CancellationToken cancellationToken)
        {
            var aggregateRoot = new AggregateRoot
            {
                AggregateAttr = request.AggregateAttr,
                LimitedDomain = request.LimitedDomain,
                LimitedService = request.LimitedService,
                EnumType1 = request.EnumType1,
                EnumType2 = request.EnumType2,
                EnumType3 = request.EnumType3,
                AggregateId = request.AggregateId,
                Composites = request.Composites
                    .Select(c => new CompositeManyB
                    {
                        CompositeAttr = c.CompositeAttr,
                        SomeDate = c.SomeDate,
                        Composites = c.Composites
                            .Select(c => new CompositeManyBB
                            {
                                CompositeAttr = c.CompositeAttr
                            })
                            .ToList()
                    })
                    .ToList()
            };

            _aggregateRootRepository.Add(aggregateRoot);
            await _aggregateRootRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return aggregateRoot.Id;
        }
    }
}