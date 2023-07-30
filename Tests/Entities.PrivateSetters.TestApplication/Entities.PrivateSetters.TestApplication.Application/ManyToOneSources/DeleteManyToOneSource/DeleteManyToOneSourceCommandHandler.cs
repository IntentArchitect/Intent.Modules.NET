using System;
using System.Threading;
using System.Threading.Tasks;
using Entities.PrivateSetters.TestApplication.Domain.Common.Exceptions;
using Entities.PrivateSetters.TestApplication.Domain.Repositories.Aggregational;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Entities.PrivateSetters.TestApplication.Application.ManyToOneSources.DeleteManyToOneSource
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteManyToOneSourceCommandHandler : IRequestHandler<DeleteManyToOneSourceCommand>
    {
        private readonly IManyToOneSourceRepository _manyToOneSourceRepository;

        [IntentManaged(Mode.Merge)]
        public DeleteManyToOneSourceCommandHandler(IManyToOneSourceRepository manyToOneSourceRepository)
        {
            _manyToOneSourceRepository = manyToOneSourceRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(DeleteManyToOneSourceCommand request, CancellationToken cancellationToken)
        {
            var existingManyToOneSource = await _manyToOneSourceRepository.FindByIdAsync(request.Id, cancellationToken);
            if (existingManyToOneSource is null)
            {
                throw new NotFoundException($"Could not find ManyToOneSource '{request.Id}'");
            }

            _manyToOneSourceRepository.Remove(existingManyToOneSource);
        }
    }
}