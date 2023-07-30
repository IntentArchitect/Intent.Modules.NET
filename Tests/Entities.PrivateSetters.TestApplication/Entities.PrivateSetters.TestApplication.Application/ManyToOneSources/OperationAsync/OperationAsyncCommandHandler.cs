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

namespace Entities.PrivateSetters.TestApplication.Application.ManyToOneSources.OperationAsync
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class OperationAsyncCommandHandler : IRequestHandler<OperationAsyncCommand>
    {
        private readonly IManyToOneSourceRepository _manyToOneSourceRepository;

        [IntentManaged(Mode.Merge)]
        public OperationAsyncCommandHandler(IManyToOneSourceRepository manyToOneSourceRepository)
        {
            _manyToOneSourceRepository = manyToOneSourceRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(OperationAsyncCommand request, CancellationToken cancellationToken)
        {
            var existingManyToOneSource = await _manyToOneSourceRepository.FindByIdAsync(request.Id, cancellationToken);
            if (existingManyToOneSource is null)
            {
                throw new NotFoundException($"Could not find ManyToOneSource '{request.Id}'");
            }

#warning No supported convention for populating "manyToOneDest" parameter
            await existingManyToOneSource.OperationAsync(request.Attribute, manyToOneDest: default, cancellationToken);
        }
    }
}