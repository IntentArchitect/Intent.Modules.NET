using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FastEndpointsTest.Domain.Common.Exceptions;
using FastEndpointsTest.Domain.Entities.CRUD;
using FastEndpointsTest.Domain.Repositories.CRUD;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace FastEndpointsTest.Application.AggregateRoots.CreateAggregateRootCompositeManyB
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateAggregateRootCompositeManyBCommandHandler : IRequestHandler<CreateAggregateRootCompositeManyBCommand, Guid>
    {
        private readonly IAggregateRootRepository _aggregateRootRepository;

        [IntentManaged(Mode.Merge)]
        public CreateAggregateRootCompositeManyBCommandHandler(IAggregateRootRepository aggregateRootRepository)
        {
            _aggregateRootRepository = aggregateRootRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(
            CreateAggregateRootCompositeManyBCommand request,
            CancellationToken cancellationToken)
        {
            var aggregateRoot = await _aggregateRootRepository.FindByIdAsync(request.AggregateRootId, cancellationToken);
            if (aggregateRoot is null)
            {
                throw new NotFoundException($"Could not find CompositeManyB '{request.AggregateRootId}'");
            }
            var compositeManyB = new CompositeManyB
            {
                CompositeAttr = request.CompositeAttr,
                SomeDate = request.SomeDate,
                AggregateRootId = request.AggregateRootId,
                Composite = request.Composite is not null
                    ? new CompositeSingleBB
                    {
                        CompositeAttr = request.Composite.CompositeAttr
                    }
                    : null,
                Composites = request.Composites
                    .Select(c => new CompositeManyBB
                    {
                        CompositeAttr = c.CompositeAttr
                    })
                    .ToList()
            };

            aggregateRoot.Composites.Add(compositeManyB);
            await _aggregateRootRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return compositeManyB.Id;
        }
    }
}