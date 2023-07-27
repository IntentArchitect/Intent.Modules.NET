using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.TestApplication.Domain.Common.Exceptions;
using CleanArchitecture.TestApplication.Domain.Repositories.Operations;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.TestApplication.Application.OperationsClasses.SyncWithReturn
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class SyncWithReturnHandler : IRequestHandler<SyncWithReturn, object>
    {
        private readonly IOperationsClassRepository _operationsClassRepository;

        [IntentManaged(Mode.Merge)]
        public SyncWithReturnHandler(IOperationsClassRepository operationsClassRepository)
        {
            _operationsClassRepository = operationsClassRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<object> Handle(SyncWithReturn request, CancellationToken cancellationToken)
        {
            var existingOperationsClass = await _operationsClassRepository.FindByIdAsync(request.Id, cancellationToken);
            if (existingOperationsClass is null)
            {
                throw new NotFoundException($"Could not find OperationsClass '{request.Id}'");
            }

            var result = existingOperationsClass.SyncWithReturn();
            return result;

        }
    }
}