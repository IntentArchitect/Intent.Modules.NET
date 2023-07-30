using System;
using System.Threading;
using System.Threading.Tasks;
using Entities.PrivateSetters.TestApplication.Domain.Common.Exceptions;
using Entities.PrivateSetters.TestApplication.Domain.Repositories.Aggregational;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Entities.PrivateSetters.TestApplication.Application.ManyToManySources.DeleteManyToManySource
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteManyToManySourceCommandHandler : IRequestHandler<DeleteManyToManySourceCommand>
    {
        private readonly IManyToManySourceRepository _manyToManySourceRepository;

        [IntentManaged(Mode.Merge)]
        public DeleteManyToManySourceCommandHandler(IManyToManySourceRepository manyToManySourceRepository)
        {
            _manyToManySourceRepository = manyToManySourceRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(DeleteManyToManySourceCommand request, CancellationToken cancellationToken)
        {
            var existingManyToManySource = await _manyToManySourceRepository.FindByIdAsync(request.Id, cancellationToken);
            if (existingManyToManySource is null)
            {
                throw new NotFoundException($"Could not find ManyToManySource '{request.Id}'");
            }

            _manyToManySourceRepository.Remove(existingManyToManySource);
        }
    }
}