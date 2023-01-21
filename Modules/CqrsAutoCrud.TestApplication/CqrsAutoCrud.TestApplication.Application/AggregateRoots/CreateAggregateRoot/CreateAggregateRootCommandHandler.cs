using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CqrsAutoCrud.TestApplication.Domain.Entities;
using CqrsAutoCrud.TestApplication.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.AggregateRoots.CreateAggregateRoot
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateAggregateRootCommandHandler : IRequestHandler<CreateAggregateRootCommand, Guid>
    {
        private readonly IAggregateRootRepository _aggregateRootRepository;

        [IntentManaged(Mode.Ignore)]
        public CreateAggregateRootCommandHandler(IAggregateRootRepository aggregateRootRepository)
        {
            _aggregateRootRepository = aggregateRootRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateAggregateRootCommand request, CancellationToken cancellationToken)
        {
            var newAggregateRoot = new AggregateRoot
            {
                AggregateAttr = request.AggregateAttr,
                Composite = request.Composite != null ? CreateCompositeSingleA(request.Composite) : null,
                Composites = request.Composites.Select(CreateCompositeManyB).ToList(),
#warning Field not a composite association: Aggregate
            };

            _aggregateRootRepository.Add(newAggregateRoot);
            await _aggregateRootRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return newAggregateRoot.Id;
        }

        [IntentManaged(Mode.Fully)]
        private CompositeSingleA CreateCompositeSingleA(CreateAggregateRootCompositeSingleADTO dto)
        {
            return new CompositeSingleA
            {
                CompositeAttr = dto.CompositeAttr,
                Composite = dto.Composite != null ? CreateCompositeSingleAA(dto.Composite) : null,
                Composites = dto.Composites.Select(CreateCompositeManyAA).ToList(),
            };
        }

        [IntentManaged(Mode.Fully)]
        private CompositeSingleAA CreateCompositeSingleAA(CreateAggregateRootCompositeSingleACompositeSingleAADTO dto)
        {
            return new CompositeSingleAA
            {
                CompositeAttr = dto.CompositeAttr,
            };
        }

        [IntentManaged(Mode.Fully)]
        private CompositeManyAA CreateCompositeManyAA(CreateAggregateRootCompositeSingleACompositeManyAADTO dto)
        {
            return new CompositeManyAA
            {
                CompositeAttr = dto.CompositeAttr,
                ACompositeSingleId = dto.ACompositeSingleId,
            };
        }

        [IntentManaged(Mode.Fully)]
        private CompositeManyB CreateCompositeManyB(CreateAggregateRootCompositeManyBDTO dto)
        {
            return new CompositeManyB
            {
                CompositeAttr = dto.CompositeAttr,
                SomeDate = dto.SomeDate,
                Composite = dto.Composite != null ? CreateCompositeSingleBB(dto.Composite) : null,
                Composites = dto.Composites.Select(CreateCompositeManyBB).ToList(),
            };
        }

        [IntentManaged(Mode.Fully)]
        private CompositeSingleBB CreateCompositeSingleBB(CreateAggregateRootCompositeManyBCompositeSingleBBDTO dto)
        {
            return new CompositeSingleBB
            {
                CompositeAttr = dto.CompositeAttr,
            };
        }

        [IntentManaged(Mode.Fully)]
        private CompositeManyBB CreateCompositeManyBB(CreateAggregateRootCompositeManyBCompositeManyBBDTO dto)
        {
            return new CompositeManyBB
            {
                CompositeAttr = dto.CompositeAttr,
                ACompositeManyId = dto.ACompositeManyId,
            };
        }
    }
}