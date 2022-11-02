using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CqrsAutoCrud.TestApplication.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.AggregateRootAS.DeleteAggregateRootA
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteAggregateRootACommandHandler : IRequestHandler<DeleteAggregateRootACommand>
    {
        private IAggregateRootARepository _aggregateRootARepository;

        [IntentManaged(Mode.Ignore)]
        public DeleteAggregateRootACommandHandler(IAggregateRootARepository aggregateRootARepository)
        {
            _aggregateRootARepository = aggregateRootARepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Unit> Handle(DeleteAggregateRootACommand request, CancellationToken cancellationToken)
        {
            var existingAggregateRootA = await _aggregateRootARepository.FindByIdAsync(request.Id, cancellationToken);
            _aggregateRootARepository.Remove(existingAggregateRootA);
            return Unit.Value;
        }
    }
}