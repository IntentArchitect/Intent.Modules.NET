using System;
using System.Threading;
using System.Threading.Tasks;
using Entities.PrivateSetters.TestApplication.Domain.Common.Exceptions;
using Entities.PrivateSetters.TestApplication.Domain.Repositories.Aggregational;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Entities.PrivateSetters.TestApplication.Application.OptionalToOneSources.DeleteOptionalToOneSource
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteOptionalToOneSourceCommandHandler : IRequestHandler<DeleteOptionalToOneSourceCommand>
    {
        private readonly IOptionalToOneSourceRepository _optionalToOneSourceRepository;

        [IntentManaged(Mode.Merge)]
        public DeleteOptionalToOneSourceCommandHandler(IOptionalToOneSourceRepository optionalToOneSourceRepository)
        {
            _optionalToOneSourceRepository = optionalToOneSourceRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(DeleteOptionalToOneSourceCommand request, CancellationToken cancellationToken)
        {
            var existingOptionalToOneSource = await _optionalToOneSourceRepository.FindByIdAsync(request.Id, cancellationToken);
            if (existingOptionalToOneSource is null)
            {
                throw new NotFoundException($"Could not find OptionalToOneSource '{request.Id}'");
            }

            _optionalToOneSourceRepository.Remove(existingOptionalToOneSource);
        }
    }
}