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

namespace CleanArchitecture.Comprehensive.Application.AggregateRoots.CreateAggregateRootCompositeManyB
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateAggregateRootCompositeManyBCommandHandler : IRequestHandler<CreateAggregateRootCompositeManyBCommand, Guid>
    {
        private readonly IAggregateRootRepository _aggregateRootRepository;

        [IntentManaged(Mode.Merge)]
        public CreateAggregateRootCompositeManyBCommandHandler(IAggregateRootRepository aggregateRootRepository)
        {
            _aggregateRootRepository = aggregateRootRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(
            CreateAggregateRootCompositeManyBCommand request,
            CancellationToken cancellationToken)
        {
            var aggregateRoot = await _aggregateRootRepository.FindByIdAsync(request.AggregateRootId, cancellationToken);
            if (aggregateRoot is null)
            {
                throw new NotFoundException($"{nameof(AggregateRoot)} of Id '{request.AggregateRootId}' could not be found");
            }

            var newCompositeManyB = new CompositeManyB
            {
                AggregateRootId = request.AggregateRootId,
                CompositeAttr = request.CompositeAttr,
                SomeDate = request.SomeDate,
                Composite = request.Composite != null ? CreateCompositeSingleBB(request.Composite) : null,
                Composites = request.Composites.Select(CreateCompositeManyBB).ToList(),
            };

            aggregateRoot.Composites.Add(newCompositeManyB);
            await _aggregateRootRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return newCompositeManyB.Id;
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
    }
}