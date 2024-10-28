using System;
using System.Threading;
using System.Threading.Tasks;
using FastEndpointsTest.Domain.Common;
using FastEndpointsTest.Domain.Common.Exceptions;
using FastEndpointsTest.Domain.Entities.CRUD;
using FastEndpointsTest.Domain.Repositories.CRUD;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace FastEndpointsTest.Application.AggregateRoots.UpdateAggregateRoot
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateAggregateRootCommandHandler : IRequestHandler<UpdateAggregateRootCommand>
    {
        private readonly IAggregateRootRepository _aggregateRootRepository;

        [IntentManaged(Mode.Merge)]
        public UpdateAggregateRootCommandHandler(IAggregateRootRepository aggregateRootRepository)
        {
            _aggregateRootRepository = aggregateRootRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateAggregateRootCommand request, CancellationToken cancellationToken)
        {
            var aggregateRoot = await _aggregateRootRepository.FindByIdAsync(request.Id, cancellationToken);
            if (aggregateRoot is null)
            {
                throw new NotFoundException($"Could not find AggregateRoot '{request.Id}'");
            }

            aggregateRoot.AggregateAttr = request.AggregateAttr;
            aggregateRoot.LimitedDomain = request.LimitedDomain;
            aggregateRoot.LimitedService = request.LimitedService;
            aggregateRoot.EnumType1 = request.EnumType1;
            aggregateRoot.EnumType2 = request.EnumType2;
            aggregateRoot.EnumType3 = request.EnumType3;
            aggregateRoot.AggregateId = request.AggregateId;
            aggregateRoot.Composites = UpdateHelper.CreateOrUpdateCollection(aggregateRoot.Composites, request.Composites, (e, d) => e.Id == d.Id, CreateOrUpdateCompositeManyB);
        }

        [IntentManaged(Mode.Fully)]
        private static CompositeManyB CreateOrUpdateCompositeManyB(
            CompositeManyB? entity,
            UpdateAggregateRootCommandCompositesDto3 dto)
        {
            entity ??= new CompositeManyB();
            entity.Id = dto.Id;
            entity.CompositeAttr = dto.CompositeAttr;
            entity.SomeDate = dto.SomeDate;
            entity.Composites = UpdateHelper.CreateOrUpdateCollection(entity.Composites, dto.Composites, (e, d) => e.Id == d.Id, CreateOrUpdateCompositeManyBB);
            return entity;
        }

        [IntentManaged(Mode.Fully)]
        private static CompositeManyBB CreateOrUpdateCompositeManyBB(
            CompositeManyBB? entity,
            UpdateAggregateRootCommandCompositesDto4 dto)
        {
            entity ??= new CompositeManyBB();
            entity.Id = dto.Id;
            entity.CompositeAttr = dto.CompositeAttr;
            return entity;
        }
    }
}