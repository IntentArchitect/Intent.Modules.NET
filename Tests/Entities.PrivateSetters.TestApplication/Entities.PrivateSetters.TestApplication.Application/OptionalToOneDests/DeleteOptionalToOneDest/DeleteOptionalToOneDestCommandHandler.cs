using System;
using System.Threading;
using System.Threading.Tasks;
using Entities.PrivateSetters.TestApplication.Domain.Common.Exceptions;
using Entities.PrivateSetters.TestApplication.Domain.Repositories.Aggregational;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Entities.PrivateSetters.TestApplication.Application.OptionalToOneDests.DeleteOptionalToOneDest
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteOptionalToOneDestCommandHandler : IRequestHandler<DeleteOptionalToOneDestCommand>
    {
        private readonly IOptionalToOneDestRepository _optionalToOneDestRepository;

        [IntentManaged(Mode.Merge)]
        public DeleteOptionalToOneDestCommandHandler(IOptionalToOneDestRepository optionalToOneDestRepository)
        {
            _optionalToOneDestRepository = optionalToOneDestRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(DeleteOptionalToOneDestCommand request, CancellationToken cancellationToken)
        {
            var existingOptionalToOneDest = await _optionalToOneDestRepository.FindByIdAsync(request.Id, cancellationToken);
            if (existingOptionalToOneDest is null)
            {
                throw new NotFoundException($"Could not find OptionalToOneDest '{request.Id}'");
            }

            _optionalToOneDestRepository.Remove(existingOptionalToOneDest);
        }
    }
}