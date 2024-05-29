using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Domain.Common;
using CleanArchitecture.Comprehensive.Domain.Common.Exceptions;
using CleanArchitecture.Comprehensive.Domain.Entities.CRUD;
using CleanArchitecture.Comprehensive.Domain.Repositories.CRUD;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.AggregateRoots.UpdateAggregateRootCompositeManyB
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
                throw new NotFoundException($"{nameof(AggregateRoot)} of Id '{request.AggregateRootId}' could not be found");
            }

            var existingCompositeManyB = aggregateRoot.Composites.FirstOrDefault(p => p.Id == request.Id);
            if (existingCompositeManyB is null)
            {
                throw new NotFoundException($"{nameof(CompositeManyB)} of Id '{request.Id}' could not be found associated with {nameof(AggregateRoot)} of Id '{request.AggregateRootId}'");
            }

            existingCompositeManyB.AggregateRootId = request.AggregateRootId;
            existingCompositeManyB.CompositeAttr = request.CompositeAttr;
            existingCompositeManyB.SomeDate = request.SomeDate;
            existingCompositeManyB.Composite = CreateOrUpdateCompositeSingleBB(existingCompositeManyB.Composite, request.Composite);
            existingCompositeManyB.Composites = UpdateHelper.CreateOrUpdateCollection(existingCompositeManyB.Composites, request.Composites, (e, d) => e.Id == d.Id, CreateOrUpdateCompositeManyBB);

        }

        [IntentManaged(Mode.Fully)]
        private static CompositeSingleBB? CreateOrUpdateCompositeSingleBB(
            CompositeSingleBB? entity,
            UpdateAggregateRootCompositeManyBCompositeSingleBBDto? dto)
        {
            if (dto == null)
            {
                return null;
            }

            entity ??= new CompositeSingleBB();
            entity.CompositeAttr = dto.CompositeAttr;

            return entity;
        }

        [IntentManaged(Mode.Fully)]
        private static CompositeManyBB CreateOrUpdateCompositeManyBB(
            CompositeManyBB? entity,
            UpdateAggregateRootCompositeManyBCompositeManyBBDto dto)
        {
            entity ??= new CompositeManyBB();
            entity.CompositeAttr = dto.CompositeAttr;
            entity.CompositeManyBId = dto.CompositeManyBId;

            return entity;
        }
    }
}