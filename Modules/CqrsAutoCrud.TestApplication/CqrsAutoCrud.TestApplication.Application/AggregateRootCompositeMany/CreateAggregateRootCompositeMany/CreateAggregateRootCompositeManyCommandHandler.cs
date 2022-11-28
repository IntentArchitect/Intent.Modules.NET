using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CqrsAutoCrud.TestApplication.Domain.Entities;
using CqrsAutoCrud.TestApplication.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.AggregateRootCompositeMany.CreateAggregateRootCompositeMany
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateAggregateRootCompositeManyCommandHandler : IRequestHandler<CreateAggregateRootCompositeManyCommand, Guid>
    {
        private readonly IAggregateRootRepository _aggregateRootRepository;

        [IntentManaged(Mode.Ignore)]
        public CreateAggregateRootCompositeManyCommandHandler(IAggregateRootRepository aggregateRootRepository)
        {
            _aggregateRootRepository = aggregateRootRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateAggregateRootCompositeManyCommand request, CancellationToken cancellationToken)
        {
            var aggregateRoot = await _aggregateRootRepository.FindByIdAsync(request.AggregateRootId, cancellationToken);
            if (aggregateRoot == null)
            {
                throw new InvalidOperationException($"{nameof(AggregateRoot)} of Id '{request.AggregateRootId}' could not be found");
            }

            var nestedEntity = new CompositeManyB
            {
                CompositeAttr = request.CompositeAttr,
                Composite = request.Composite != null ? CreateCompositeCompositeSingleBB(request.Composite) : null,
                Composites = request.Composites.Select(CreateCompositesCompositeManyBB).ToList(),
            };
            aggregateRoot.Composites.Add(nestedEntity);
            
            await _aggregateRootRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return nestedEntity.Id;
        }

       

        [IntentManaged(Mode.Fully)]
        private CompositeManyB CreateCompositesCompositeManyB(CreateCompositeManyBDTO dto)
        {
            return new CompositeManyB
            {
                CompositeAttr = dto.CompositeAttr,
                AAggregaterootId = dto.AAggregaterootId,
                Composite = dto.Composite != null ? CreateCompositeCompositeSingleBB(dto.Composite) : null,
                Composites = dto.Composites.Select(CreateCompositesCompositeManyBB).ToList(),
            };
        }

        [IntentManaged(Mode.Fully)]
        private CompositeSingleBB CreateCompositeCompositeSingleBB(CreateCompositeSingleBBDTO dto)
        {
            return new CompositeSingleBB
            {
                CompositeAttr = dto.CompositeAttr,
            };
        }

        [IntentManaged(Mode.Fully)]
        private CompositeManyBB CreateCompositesCompositeManyBB(CreateCompositeManyBBDTO dto)
        {
            return new CompositeManyBB
            {
                CompositeAttr = dto.CompositeAttr,
                ACompositeManyId = dto.ACompositeManyId,
            };
        }
    }

}