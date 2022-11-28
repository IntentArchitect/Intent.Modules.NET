using System.Threading;
using System.Threading.Tasks;
using CqrsAutoCrud.TestApplication.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.AggregateRootCompositeSingleAs.DeleteAggregateRootCompositeSingleA
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteAggregateRootCompositeSingleACommandHandler : IRequestHandler<DeleteAggregateRootCompositeSingleACommand>
    {
        private readonly IAggregateRootRepository _aggregateRootRepository;

        [IntentManaged(Mode.Ignore)]
        public DeleteAggregateRootCompositeSingleACommandHandler(IAggregateRootRepository aggregateRootRepository)
        {
            _aggregateRootRepository = aggregateRootRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Unit> Handle(DeleteAggregateRootCompositeSingleACommand request, CancellationToken cancellationToken)
        {
            var existingAggregateRoot = await _aggregateRootRepository.FindByIdAsync(request.AggregateRootId, cancellationToken);
            existingAggregateRoot.Composite = null;
            return Unit.Value;
        }
    }
}