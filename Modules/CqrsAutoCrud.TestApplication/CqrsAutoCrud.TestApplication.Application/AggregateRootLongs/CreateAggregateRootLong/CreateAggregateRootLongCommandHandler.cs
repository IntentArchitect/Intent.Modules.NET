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

namespace CqrsAutoCrud.TestApplication.Application.AggregateRootLongs.CreateAggregateRootLong
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateAggregateRootLongCommandHandler : IRequestHandler<CreateAggregateRootLongCommand, long>
    {
        private IAggregateRootLongRepository _aggregateRootLongRepository;

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
                CompositeOfAggrLong = request.CompositeOfAggrLong != null
                    ? new CompositeOfAggrLong
                    {
                        Attribute = request.CompositeOfAggrLong.Attribute,
                    }
                    : null,
            };

            _aggregateRootLongRepository.Add(newAggregateRootLong);

            await _aggregateRootLongRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return newAggregateRootLong.Id;
        }
    }
}