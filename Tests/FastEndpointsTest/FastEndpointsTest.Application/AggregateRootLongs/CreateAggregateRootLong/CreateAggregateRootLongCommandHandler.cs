using System;
using System.Threading;
using System.Threading.Tasks;
using FastEndpointsTest.Domain.Entities.CRUD;
using FastEndpointsTest.Domain.Repositories.CRUD;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace FastEndpointsTest.Application.AggregateRootLongs.CreateAggregateRootLong
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateAggregateRootLongCommandHandler : IRequestHandler<CreateAggregateRootLongCommand, long>
    {
        private readonly IAggregateRootLongRepository _aggregateRootLongRepository;

        [IntentManaged(Mode.Merge)]
        public CreateAggregateRootLongCommandHandler(IAggregateRootLongRepository aggregateRootLongRepository)
        {
            _aggregateRootLongRepository = aggregateRootLongRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<long> Handle(CreateAggregateRootLongCommand request, CancellationToken cancellationToken)
        {
            var aggregateRootLong = new AggregateRootLong
            {
                Attribute = request.Attribute,
                CompositeOfAggrLong = request.CompositeOfAggrLong is not null
                    ? new CompositeOfAggrLong
                    {
                        Attribute = request.CompositeOfAggrLong.Attribute
                    }
                    : null
            };

            _aggregateRootLongRepository.Add(aggregateRootLong);
            await _aggregateRootLongRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return aggregateRootLong.Id;
        }
    }
}