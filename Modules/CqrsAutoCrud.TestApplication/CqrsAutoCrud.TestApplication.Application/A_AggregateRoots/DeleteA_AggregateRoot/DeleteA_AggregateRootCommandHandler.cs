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

namespace CqrsAutoCrud.TestApplication.Application.A_AggregateRoots.DeleteA_AggregateRoot
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteA_AggregateRootCommandHandler : IRequestHandler<DeleteA_AggregateRootCommand>
    {
        private IA_AggregateRootRepository _a_AggregateRootRepository;

        [IntentManaged(Mode.Ignore)]
        public DeleteA_AggregateRootCommandHandler(IA_AggregateRootRepository a_AggregateRootRepository)
        {
            _a_AggregateRootRepository = a_AggregateRootRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Unit> Handle(DeleteA_AggregateRootCommand request, CancellationToken cancellationToken)
        {
            var existingA_AggregateRoot = await _a_AggregateRootRepository.FindByIdAsync(request.Id, cancellationToken);
            _a_AggregateRootRepository.Remove(existingA_AggregateRoot);
            return Unit.Value;
        }
    }
}