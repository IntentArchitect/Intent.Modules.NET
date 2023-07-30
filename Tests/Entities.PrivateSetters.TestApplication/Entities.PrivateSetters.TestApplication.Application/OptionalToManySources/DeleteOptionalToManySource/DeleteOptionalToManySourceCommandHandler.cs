using System;
using System.Threading;
using System.Threading.Tasks;
using Entities.PrivateSetters.TestApplication.Domain.Common.Exceptions;
using Entities.PrivateSetters.TestApplication.Domain.Repositories.Aggregational;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Entities.PrivateSetters.TestApplication.Application.OptionalToManySources.DeleteOptionalToManySource
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteOptionalToManySourceCommandHandler : IRequestHandler<DeleteOptionalToManySourceCommand>
    {
        private readonly IOptionalToManySourceRepository _optionalToManySourceRepository;

        [IntentManaged(Mode.Merge)]
        public DeleteOptionalToManySourceCommandHandler(IOptionalToManySourceRepository optionalToManySourceRepository)
        {
            _optionalToManySourceRepository = optionalToManySourceRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(DeleteOptionalToManySourceCommand request, CancellationToken cancellationToken)
        {
            var existingOptionalToManySource = await _optionalToManySourceRepository.FindByIdAsync(request.Id, cancellationToken);
            if (existingOptionalToManySource is null)
            {
                throw new NotFoundException($"Could not find OptionalToManySource '{request.Id}'");
            }

            _optionalToManySourceRepository.Remove(existingOptionalToManySource);
        }
    }
}