using System;
using System.Threading;
using System.Threading.Tasks;
using IntegrationTesting.Tests.Domain.Common.Exceptions;
using IntegrationTesting.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.DiffIds.UpdateDiffId
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateDiffIdCommandHandler : IRequestHandler<UpdateDiffIdCommand>
    {
        private readonly IDiffIdRepository _diffIdRepository;

        [IntentManaged(Mode.Merge)]
        public UpdateDiffIdCommandHandler(IDiffIdRepository diffIdRepository)
        {
            _diffIdRepository = diffIdRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateDiffIdCommand request, CancellationToken cancellationToken)
        {
            var diffId = await _diffIdRepository.FindByIdAsync(request.MyId, cancellationToken);
            if (diffId is null)
            {
                throw new NotFoundException($"Could not find DiffId '{request.MyId}'");
            }

            diffId.Name = request.Name;
        }
    }
}