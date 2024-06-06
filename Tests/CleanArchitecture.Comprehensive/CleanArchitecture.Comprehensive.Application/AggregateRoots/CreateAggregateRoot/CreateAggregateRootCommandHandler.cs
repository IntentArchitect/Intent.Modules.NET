using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Domain.Entities.CRUD;
using CleanArchitecture.Comprehensive.Domain.Repositories.CRUD;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.AggregateRoots.CreateAggregateRoot
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateAggregateRootCommandHandler : IRequestHandler<CreateAggregateRootCommand, Guid>
    {
        private readonly IAggregateRootRepository _aggregateRootRepository;

        [IntentManaged(Mode.Merge)]
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
                Composites = request.Composites.Select(CreateCompositeManyB).ToList(),
                Composite = request.Composite != null ? CreateCompositeSingleA(request.Composite) : null,
#warning Field not a composite association: Aggregate
                LimitedDomain = request.LimitedDomain,
                LimitedService = request.LimitedService,
            };

            _aggregateRootRepository.Add(newAggregateRoot);
            await _aggregateRootRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return newAggregateRoot.Id;
        }

        [IntentManaged(Mode.Fully)]
        private static CompositeManyB CreateCompositeManyB(CreateAggregateRootCompositeManyBDto dto)
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
        private static CompositeSingleBB CreateCompositeSingleBB(CreateAggregateRootCompositeManyBCompositeSingleBBDto dto)
        {
            return new CompositeSingleBB
            {
                CompositeAttr = dto.CompositeAttr,
            };
        }

        [IntentManaged(Mode.Fully)]
        private static CompositeManyBB CreateCompositeManyBB(CreateAggregateRootCompositeManyBCompositeManyBBDto dto)
        {
            return new CompositeManyBB
            {
                CompositeAttr = dto.CompositeAttr,
            };
        }

        [IntentManaged(Mode.Fully)]
        private static CompositeSingleA CreateCompositeSingleA(CreateAggregateRootCompositeSingleADto dto)
        {
            return new CompositeSingleA
            {
                CompositeAttr = dto.CompositeAttr,
                Composite = dto.Composite != null ? CreateCompositeSingleAA(dto.Composite) : null,
                Composites = dto.Composites.Select(CreateCompositeManyAA).ToList(),
            };
        }

        [IntentManaged(Mode.Fully)]
        private static CompositeSingleAA CreateCompositeSingleAA(CreateAggregateRootCompositeSingleACompositeSingleAADto dto)
        {
            return new CompositeSingleAA
            {
                CompositeAttr = dto.CompositeAttr,
            };
        }

        [IntentManaged(Mode.Fully)]
        private static CompositeManyAA CreateCompositeManyAA(CreateAggregateRootCompositeSingleACompositeManyAADto dto)
        {
            return new CompositeManyAA
            {
                CompositeAttr = dto.CompositeAttr,
            };
        }
    }
}