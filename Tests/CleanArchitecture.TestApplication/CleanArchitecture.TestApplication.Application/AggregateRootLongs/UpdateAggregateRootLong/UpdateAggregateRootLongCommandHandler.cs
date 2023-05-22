using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.TestApplication.Domain.Common;
using CleanArchitecture.TestApplication.Domain.Entities;
using CleanArchitecture.TestApplication.Domain.Entities.CRUD;
using CleanArchitecture.TestApplication.Domain.Repositories;
using CleanArchitecture.TestApplication.Domain.Repositories.CRUD;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.AggregateRootLongs.UpdateAggregateRootLong
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateAggregateRootLongCommandHandler : IRequestHandler<UpdateAggregateRootLongCommand>
    {
        private readonly IAggregateRootLongRepository _aggregateRootLongRepository;

        [IntentManaged(Mode.Ignore)]
        public UpdateAggregateRootLongCommandHandler(IAggregateRootLongRepository aggregateRootLongRepository)
        {
            _aggregateRootLongRepository = aggregateRootLongRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Unit> Handle(UpdateAggregateRootLongCommand request, CancellationToken cancellationToken)
        {
            var existingAggregateRootLong = await _aggregateRootLongRepository.FindByIdAsync(request.Id, cancellationToken);
            existingAggregateRootLong.Attribute = request.Attribute;
            existingAggregateRootLong.CompositeOfAggrLong = CreateOrUpdateCompositeOfAggrLong(existingAggregateRootLong.CompositeOfAggrLong, request.CompositeOfAggrLong);
            return Unit.Value;
        }

        [IntentManaged(Mode.Fully)]
        private static CompositeOfAggrLong CreateOrUpdateCompositeOfAggrLong(
            CompositeOfAggrLong entity,
            UpdateAggregateRootLongCompositeOfAggrLongDto dto)
        {

            entity ??= new CompositeOfAggrLong();
            entity.Attribute = dto.Attribute;

            return entity;
        }
    }
}