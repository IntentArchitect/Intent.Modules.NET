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

namespace Entities.PrivateSetters.TestApplication.Application.OptionalToOneDests.OperationAsync
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class OperationAsyncCommandHandler : IRequestHandler<OperationAsyncCommand>
    {
        private readonly IOptionalToOneDestRepository _optionalToOneDestRepository;

        [IntentManaged(Mode.Merge)]
        public OperationAsyncCommandHandler(IOptionalToOneDestRepository optionalToOneDestRepository)
        {
            _optionalToOneDestRepository = optionalToOneDestRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(OperationAsyncCommand request, CancellationToken cancellationToken)
        {
            var existingOptionalToOneDest = await _optionalToOneDestRepository.FindByIdAsync(request.Id, cancellationToken);
            if (existingOptionalToOneDest is null)
            {
                throw new NotFoundException($"Could not find OptionalToOneDest '{request.Id}'");
            }

#warning No supported convention for populating "optionalToOneSource" parameter
            await existingOptionalToOneDest.OperationAsync(request.Attribute, optionalToOneSource: default, cancellationToken);
        }
    }
}