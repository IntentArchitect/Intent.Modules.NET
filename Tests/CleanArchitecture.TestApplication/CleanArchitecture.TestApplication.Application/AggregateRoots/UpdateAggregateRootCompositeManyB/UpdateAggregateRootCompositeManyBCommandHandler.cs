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

namespace CleanArchitecture.TestApplication.Application.AggregateRoots.UpdateAggregateRootCompositeManyB
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateAggregateRootCompositeManyBCommandHandler : IRequestHandler<UpdateAggregateRootCompositeManyBCommand>
    {
        private readonly IAggregateRootRepository _aggregateRootRepository;

        [IntentManaged(Mode.Ignore)]
        public UpdateAggregateRootCompositeManyBCommandHandler(IAggregateRootRepository aggregateRootRepository)
        {
            _aggregateRootRepository = aggregateRootRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Unit> Handle(
            UpdateAggregateRootCompositeManyBCommand request,
            CancellationToken cancellationToken)
        {
            var aggregateRoot = await _aggregateRootRepository.FindByIdAsync(request.AggregateRootId, cancellationToken);
            if (aggregateRoot == null)
            {
                throw new InvalidOperationException($"{nameof(AggregateRoot)} of Id '{request.AggregateRootId}' could not be found");
            }
            var element = aggregateRoot.Composites.FirstOrDefault(p => p.Id == request.Id);
            if (element == null)
            {
                throw new InvalidOperationException($"{nameof(CompositeManyB)} of Id '{request.Id}' could not be found associated with {nameof(AggregateRoot)} of Id '{request.AggregateRootId}'");
            }
            element.AggregateRootId = request.AggregateRootId;
            element.CompositeAttr = request.CompositeAttr;
            element.SomeDate = request.SomeDate;
            element.Composite = CreateOrUpdateCompositeSingleBB(element.Composite, request.Composite);
            element.Composites = UpdateHelper.CreateOrUpdateCollection(element.Composites, request.Composites, (e, d) => e.Id == d.Id, CreateOrUpdateCompositeManyBB);
            return Unit.Value;
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
            CompositeManyBB entity,
            UpdateAggregateRootCompositeManyBCompositeManyBBDto dto)
        {

            entity ??= new CompositeManyBB();
            entity.CompositeAttr = dto.CompositeAttr;
            entity.CompositeManyBId = dto.CompositeManyBId;

            return entity;
        }
    }
}