using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Domain.Entities.CRUD;
using CleanArchitecture.Comprehensive.Domain.Repositories.CRUD;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.AggregateRootLongs.CreateAggregateRootLong
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
        private static CompositeOfAggrLong CreateCompositeOfAggrLong(CreateAggregateRootLongCompositeOfAggrLongDto dto)
        {
            return new CompositeOfAggrLong
            {
                Attribute = dto.Attribute,
            };
        }
    }
}