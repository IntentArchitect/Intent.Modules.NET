using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Domain.Common.Exceptions;
using CleanArchitecture.Comprehensive.Domain.Repositories.Operations;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.OperationsClasses.Sync
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
        public async Task Handle(Sync request, CancellationToken cancellationToken)
        {
            var existingOperationsClass = await _operationsClassRepository.FindByIdAsync(request.Id, cancellationToken);
            if (existingOperationsClass is null)
            {
                throw new NotFoundException($"Could not find OperationsClass '{request.Id}'");
            }

            existingOperationsClass.Sync();

        }
    }
}