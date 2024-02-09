using System;
using System.Threading;
using System.Threading.Tasks;
using IntegrationTesting.Tests.Domain.Common.Exceptions;
using IntegrationTesting.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.DiffIds.DeleteDiffId
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteDiffIdCommandHandler : IRequestHandler<DeleteDiffIdCommand>
    {
        private readonly IDiffIdRepository _diffIdRepository;

        [IntentManaged(Mode.Merge)]
        public DeleteDiffIdCommandHandler(IDiffIdRepository diffIdRepository)
        {
            _diffIdRepository = diffIdRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(DeleteDiffIdCommand request, CancellationToken cancellationToken)
        {
            var diffId = await _diffIdRepository.FindByIdAsync(request.MyId, cancellationToken);
            if (diffId is null)
            {
                throw new NotFoundException($"Could not find DiffId '{request.MyId}'");
            }

            _diffIdRepository.Remove(diffId);
        }
    }
}