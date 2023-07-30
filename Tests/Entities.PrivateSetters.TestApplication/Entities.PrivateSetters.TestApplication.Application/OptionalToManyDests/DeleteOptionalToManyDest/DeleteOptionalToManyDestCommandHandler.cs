using System;
using System.Threading;
using System.Threading.Tasks;
using Entities.PrivateSetters.TestApplication.Domain.Common.Exceptions;
using Entities.PrivateSetters.TestApplication.Domain.Repositories.Aggregational;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Entities.PrivateSetters.TestApplication.Application.OptionalToManyDests.DeleteOptionalToManyDest
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteOptionalToManyDestCommandHandler : IRequestHandler<DeleteOptionalToManyDestCommand>
    {
        private readonly IOptionalToManyDestRepository _optionalToManyDestRepository;

        [IntentManaged(Mode.Merge)]
        public DeleteOptionalToManyDestCommandHandler(IOptionalToManyDestRepository optionalToManyDestRepository)
        {
            _optionalToManyDestRepository = optionalToManyDestRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(DeleteOptionalToManyDestCommand request, CancellationToken cancellationToken)
        {
            var existingOptionalToManyDest = await _optionalToManyDestRepository.FindByIdAsync(request.Id, cancellationToken);
            if (existingOptionalToManyDest is null)
            {
                throw new NotFoundException($"Could not find OptionalToManyDest '{request.Id}'");
            }

            _optionalToManyDestRepository.Remove(existingOptionalToManyDest);
        }
    }
}