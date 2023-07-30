using System;
using System.Threading;
using System.Threading.Tasks;
using Entities.PrivateSetters.TestApplication.Domain.Common.Exceptions;
using Entities.PrivateSetters.TestApplication.Domain.Repositories.Aggregational;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Entities.PrivateSetters.TestApplication.Application.ManyToOneDests.DeleteManyToOneDest
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteManyToOneDestCommandHandler : IRequestHandler<DeleteManyToOneDestCommand>
    {
        private readonly IManyToOneDestRepository _manyToOneDestRepository;

        [IntentManaged(Mode.Merge)]
        public DeleteManyToOneDestCommandHandler(IManyToOneDestRepository manyToOneDestRepository)
        {
            _manyToOneDestRepository = manyToOneDestRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(DeleteManyToOneDestCommand request, CancellationToken cancellationToken)
        {
            var existingManyToOneDest = await _manyToOneDestRepository.FindByIdAsync(request.Id, cancellationToken);
            if (existingManyToOneDest is null)
            {
                throw new NotFoundException($"Could not find ManyToOneDest '{request.Id}'");
            }

            _manyToOneDestRepository.Remove(existingManyToOneDest);
        }
    }
}