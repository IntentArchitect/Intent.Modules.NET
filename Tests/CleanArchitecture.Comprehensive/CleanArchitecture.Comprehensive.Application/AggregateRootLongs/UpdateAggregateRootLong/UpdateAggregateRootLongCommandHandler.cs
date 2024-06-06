using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Domain.Common.Exceptions;
using CleanArchitecture.Comprehensive.Domain.Entities.CRUD;
using CleanArchitecture.Comprehensive.Domain.Repositories.CRUD;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.AggregateRootLongs.UpdateAggregateRootLong
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
            var existingAggregateRootLong = await _aggregateRootLongRepository.FindByIdAsync(request.Id, cancellationToken);
            if (existingAggregateRootLong is null)
            {
                throw new NotFoundException($"Could not find AggregateRootLong '{request.Id}'");
            }

            existingAggregateRootLong.Attribute = request.Attribute;
            existingAggregateRootLong.CompositeOfAggrLong = CreateOrUpdateCompositeOfAggrLong(existingAggregateRootLong.CompositeOfAggrLong, request.CompositeOfAggrLong);

        }

        [IntentManaged(Mode.Fully)]
        private static CompositeOfAggrLong? CreateOrUpdateCompositeOfAggrLong(
            CompositeOfAggrLong? entity,
            UpdateAggregateRootLongCompositeOfAggrLongDto? dto)
        {
            if (dto == null)
            {
                return null;
            }

            entity ??= new CompositeOfAggrLong();
            entity.Attribute = dto.Attribute;

            return entity;
        }
    }
}