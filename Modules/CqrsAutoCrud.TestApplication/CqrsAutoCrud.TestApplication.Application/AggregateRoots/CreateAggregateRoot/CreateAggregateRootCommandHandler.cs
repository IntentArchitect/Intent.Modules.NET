using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CqrsAutoCrud.TestApplication.Domain.Entities;
using CqrsAutoCrud.TestApplication.Domain.Entities.Common;
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
        private IAggregateRootRepository _aggregateRootRepository;

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
                Composite = request.Composite != null
                    ? CreateCompositeSingleA(request.Composite)
                    : null,
                Composites = request.Composites.Select(CreateCompositeManyB).ToList(),
#warning Property not a composite association: Aggregate
            };

            _aggregateRootRepository.Add(newAggregateRoot);

            await _aggregateRootRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return newAggregateRoot.Id;
        }

        private static CompositeManyB CreateCompositeManyB(CreateCompositeManyBDTO dto)
        {
            return new CompositeManyB
            {
                CompositeAttr = dto.CompositeAttr,
                AAggregaterootId = dto.AAggregaterootId,
                Composite = dto.Composite != null
                    ? CreateCompositeSingleBB(dto)
                    : null,
                Composites = dto.Composites.Select(CreateCompositeManyBB).ToList(),
            };
        }

        private static CompositeManyBB CreateCompositeManyBB(CreateCompositeManyBBDTO dto)
        {
            return new CompositeManyBB
            {
                CompositeAttr = dto.CompositeAttr,
                ACompositeManyId = dto.ACompositeManyId,
            };
        }

        private static CompositeSingleBB CreateCompositeSingleBB(CreateCompositeManyBDTO dto)
        {
            return new CompositeSingleBB
            {
                CompositeAttr = dto.Composite.CompositeAttr,
            };
        }

        private static CompositeSingleA CreateCompositeSingleA(CreateCompositeSingleADTO dto)
        {
            return new CompositeSingleA
            {
                CompositeAttr = dto.CompositeAttr,
                Composite = dto.Composite != null
                    ? CreateCompositeSingleAA(dto.Composite)
                    : null,
                Composites = dto.Composites.Select(CreateCompositeManyAA).ToList(),
            };
        }

        private static CompositeSingleAA CreateCompositeSingleAA(CreateCompositeSingleAADTO dto)
        {
            return new CompositeSingleAA
            {
                CompositeAttr = dto.CompositeAttr,
            };
        }

        private static CompositeManyAA CreateCompositeManyAA(CreateCompositeManyAADTO dto)
        {
            return new CompositeManyAA
            {
                CompositeAttr = dto.CompositeAttr,
                ACompositeSingleId = dto.ACompositeSingleId,
            };
        }
    }
}