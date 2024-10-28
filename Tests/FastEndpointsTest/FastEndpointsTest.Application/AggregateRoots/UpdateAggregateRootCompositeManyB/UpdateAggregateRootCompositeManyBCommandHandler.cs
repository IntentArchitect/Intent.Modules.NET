using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FastEndpointsTest.Domain.Common.Exceptions;
using FastEndpointsTest.Domain.Repositories.CRUD;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace FastEndpointsTest.Application.AggregateRoots.UpdateAggregateRootCompositeManyB
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateAggregateRootCompositeManyBCommandHandler : IRequestHandler<UpdateAggregateRootCompositeManyBCommand>
    {
        private readonly IAggregateRootRepository _aggregateRootRepository;

        [IntentManaged(Mode.Merge)]
        public UpdateAggregateRootCompositeManyBCommandHandler(IAggregateRootRepository aggregateRootRepository)
        {
            _aggregateRootRepository = aggregateRootRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateAggregateRootCompositeManyBCommand request, CancellationToken cancellationToken)
        {
            var aggregateRoot = await _aggregateRootRepository.FindByIdAsync(request.AggregateRootId, cancellationToken);
            if (aggregateRoot is null)
            {
                throw new NotFoundException($"Could not find CompositeManyB '{request.AggregateRootId}'");
            }

            var compositeManyB = aggregateRoot.Composites.FirstOrDefault(x => x.Id == request.Id);
            if (compositeManyB is null)
            {
                throw new NotFoundException($"Could not find CompositeManyB '{request.Id}'");
            }

            compositeManyB.CompositeAttr = request.CompositeAttr;
            compositeManyB.SomeDate = request.SomeDate;
            compositeManyB.AggregateRootId = request.AggregateRootId;
        }
    }
}