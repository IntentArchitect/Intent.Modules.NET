using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Entities.PrivateSetters.TestApplication.Domain.Common.Exceptions;
using Entities.PrivateSetters.TestApplication.Domain.Repositories.Aggregational;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Entities.PrivateSetters.TestApplication.Application.ManyToOneDests.OperationAsync
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class OperationAsyncCommandHandler : IRequestHandler<OperationAsyncCommand>
    {
        private readonly IManyToOneDestRepository _manyToOneDestRepository;

        [IntentManaged(Mode.Merge)]
        public OperationAsyncCommandHandler(IManyToOneDestRepository manyToOneDestRepository)
        {
            _manyToOneDestRepository = manyToOneDestRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(OperationAsyncCommand request, CancellationToken cancellationToken)
        {
            var existingManyToOneDest = await _manyToOneDestRepository.FindByIdAsync(request.Id, cancellationToken);
            if (existingManyToOneDest is null)
            {
                throw new NotFoundException($"Could not find ManyToOneDest '{request.Id}'");
            }

            await existingManyToOneDest.OperationAsync(request.Attribute, cancellationToken);
        }
    }
}