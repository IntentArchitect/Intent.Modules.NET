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

namespace CqrsAutoCrud.TestApplication.Application.AggregateRootCompositeManyBS.CreateAggregateRootCompositeManyB
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateAggregateRootCompositeManyBCommandHandler : IRequestHandler<CreateAggregateRootCompositeManyBCommand, Guid>
    {
        private readonly IAggregateRootRepository _aggregateRootRepository;

        [IntentManaged(Mode.Ignore)]
        public CreateAggregateRootCompositeManyBCommandHandler(IAggregateRootRepository aggregateRootRepository)
        {
            _aggregateRootRepository = aggregateRootRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateAggregateRootCompositeManyBCommand request, CancellationToken cancellationToken)
        {
            var aggregateRoot = await _aggregateRootRepository.FindByIdAsync(request.AggregateRootId, cancellationToken);
            if (aggregateRoot == null)
            {
                throw new InvalidOperationException($"{nameof(AggregateRoot)} of Id '{request.AggregateRootId}' could not be found");
            }
            var newCompositeManyB = new CompositeManyB
            {
                AggregateRootId = request.AggregateRootId,
                CompositeAttr = request.CompositeAttr,
                SomeDate = request.SomeDate,
            };

            aggregateRoot.Composites.Add(newCompositeManyB);
            await _aggregateRootRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return newCompositeManyB.Id;
        }
    }
}