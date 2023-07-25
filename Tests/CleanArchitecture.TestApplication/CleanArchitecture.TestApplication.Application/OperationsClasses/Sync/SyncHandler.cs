using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.TestApplication.Domain.Common.Exceptions;
using CleanArchitecture.TestApplication.Domain.Repositories.Operations;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.OperationsClasses.Sync
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class SyncHandler : IRequestHandler<Sync>
    {
        private readonly IOperationsClassRepository _operationsClassRepository;

        [IntentManaged(Mode.Merge)]
        public SyncHandler(IOperationsClassRepository operationsClassRepository)
        {
            _operationsClassRepository = operationsClassRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Unit> Handle(Sync request, CancellationToken cancellationToken)
        {
            var entity = await _operationsClassRepository.FindByIdAsync(request.Id, cancellationToken);
            if (entity is null)
            {
                throw new NotFoundException($"Could not find OperationsClass '{request.Id}'");
            }

            entity.Sync();
            return Unit.Value;
        }
    }
}