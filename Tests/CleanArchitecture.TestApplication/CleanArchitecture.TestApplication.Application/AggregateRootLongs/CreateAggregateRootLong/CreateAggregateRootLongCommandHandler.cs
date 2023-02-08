using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.TestApplication.Domain.Entities;
using CleanArchitecture.TestApplication.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.AggregateRootLongs.CreateAggregateRootLong
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateAggregateRootLongCommandHandler : IRequestHandler<CreateAggregateRootLongCommand, long>
    {
        private readonly IAggregateRootLongRepository _aggregateRootLongRepository;

        [IntentManaged(Mode.Ignore)]
        public CreateAggregateRootLongCommandHandler(IAggregateRootLongRepository aggregateRootLongRepository)
        {
            _aggregateRootLongRepository = aggregateRootLongRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<long> Handle(CreateAggregateRootLongCommand request, CancellationToken cancellationToken)
        {
            var newAggregateRootLong = new AggregateRootLong
            {
                Attribute = request.Attribute,
                CompositeOfAggrLong = request.CompositeOfAggrLong != null ? CreateCompositeOfAggrLong(request.CompositeOfAggrLong) : null,
            };

            _aggregateRootLongRepository.Add(newAggregateRootLong);
            await _aggregateRootLongRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return newAggregateRootLong.Id;
        }

        [IntentManaged(Mode.Fully)]
        private CompositeOfAggrLong CreateCompositeOfAggrLong(CreateAggregateRootLongCompositeOfAggrLongDto dto)
        {
            return new CompositeOfAggrLong
            {
                Attribute = dto.Attribute,
            };
        }
    }
}