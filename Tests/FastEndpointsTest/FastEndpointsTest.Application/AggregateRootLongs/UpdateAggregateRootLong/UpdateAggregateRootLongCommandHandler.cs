using System;
using System.Threading;
using System.Threading.Tasks;
using FastEndpointsTest.Domain.Common.Exceptions;
using FastEndpointsTest.Domain.Entities.CRUD;
using FastEndpointsTest.Domain.Repositories.CRUD;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace FastEndpointsTest.Application.AggregateRootLongs.UpdateAggregateRootLong
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateAggregateRootLongCommandHandler : IRequestHandler<UpdateAggregateRootLongCommand>
    {
        private readonly IAggregateRootLongRepository _aggregateRootLongRepository;

        [IntentManaged(Mode.Merge)]
        public UpdateAggregateRootLongCommandHandler(IAggregateRootLongRepository aggregateRootLongRepository)
        {
            _aggregateRootLongRepository = aggregateRootLongRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateAggregateRootLongCommand request, CancellationToken cancellationToken)
        {
            var aggregateRootLong = await _aggregateRootLongRepository.FindByIdAsync(request.Id, cancellationToken);
            if (aggregateRootLong is null)
            {
                throw new NotFoundException($"Could not find AggregateRootLong '{request.Id}'");
            }

            aggregateRootLong.Attribute = request.Attribute;
            aggregateRootLong.CompositeOfAggrLong ??= new CompositeOfAggrLong();
            aggregateRootLong.CompositeOfAggrLong.Attribute = request.CompositeOfAggrLong.Attribute;
        }
    }
}