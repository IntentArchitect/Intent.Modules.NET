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

namespace CqrsAutoCrud.TestApplication.Application.AggregateRootCompositeSingleAs.CreateAggregateRootCompositeSingleA
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateAggregateRootCompositeSingleACommandHandler : IRequestHandler<CreateAggregateRootCompositeSingleACommand, Guid>
    {
        private readonly IAggregateRootRepository _aggregateRootRepository;

        [IntentManaged(Mode.Ignore)]
        public CreateAggregateRootCompositeSingleACommandHandler(IAggregateRootRepository aggregateRootRepository)
        {
            _aggregateRootRepository = aggregateRootRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateAggregateRootCompositeSingleACommand request, CancellationToken cancellationToken)
        {
            var aggregateRoot = await _aggregateRootRepository.FindByIdAsync(request.AggregateRootId, cancellationToken);
            if (aggregateRoot == null)
            {
                throw new InvalidOperationException($"{nameof(AggregateRoot)} of Id '{request.AggregateRootId}' could not be found");
            }
            
            var nestedEntity = new CompositeSingleA
            {
                CompositeAttr = request.CompositeAttr,
                Composite = request.Composite != null ? CreateCompositeCompositeSingleAA(request.Composite) : null,
                Composites = request.Composites.Select(CreateCompositesCompositeManyAA).ToList(),
            };
            aggregateRoot.Composite = nestedEntity;

            await _aggregateRootRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return nestedEntity.Id;
        }

        [IntentManaged(Mode.Fully)]
        private CompositeSingleAA CreateCompositeCompositeSingleAA(CreateCompositeSingleAADTO dto)
        {
            return new CompositeSingleAA
            {
                CompositeAttr = dto.CompositeAttr,
            };
        }

        [IntentManaged(Mode.Fully)]
        private CompositeManyAA CreateCompositesCompositeManyAA(CreateCompositeManyAADTO dto)
        {
            return new CompositeManyAA
            {
                CompositeAttr = dto.CompositeAttr,
                ACompositeSingleId = dto.ACompositeSingleId,
            };
        }
    }
}