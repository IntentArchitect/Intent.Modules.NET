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
                Composite = request.Composite != null ? CreateCompositeCompositeSingleA(request.Composite) : null,
                Composites = request.Composites.Select(CreateCompositesCompositeManyB).ToList(),
#warning Field not a composite association: Aggregate
            };

            _aggregateRootRepository.Add(newAggregateRoot);
            await _aggregateRootRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return newAggregateRoot.Id;
        }

        [IntentManaged(Mode.Fully)]
        private CompositeSingleA CreateCompositeCompositeSingleA(CreateCompositeSingleADTO dto)
        {
            return new CompositeSingleA
            {
                CompositeAttr = dto.CompositeAttr,
                Composite = dto.Composite != null ? CreateCompositeCompositeSingleAA(dto.Composite) : null,
                Composites = dto.Composites.Select(CreateCompositesCompositeManyAA).ToList(),
            };
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

        [IntentManaged(Mode.Fully)]
        private CompositeManyB CreateCompositesCompositeManyB(CreateCompositeManyBDTO dto)
        {
            return new CompositeManyB
            {
                CompositeAttr = dto.CompositeAttr,
                AggregateRootId = dto.AAggregaterootId,
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