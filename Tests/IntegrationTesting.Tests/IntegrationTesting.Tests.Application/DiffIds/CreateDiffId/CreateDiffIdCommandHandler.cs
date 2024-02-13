using System;
using System.Threading;
using System.Threading.Tasks;
using IntegrationTesting.Tests.Domain.Entities;
using IntegrationTesting.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.DiffIds.CreateDiffId
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateDiffIdCommandHandler : IRequestHandler<CreateDiffIdCommand, Guid>
    {
        private readonly IDiffIdRepository _diffIdRepository;

        [IntentManaged(Mode.Merge)]
        public CreateDiffIdCommandHandler(IDiffIdRepository diffIdRepository)
        {
            _diffIdRepository = diffIdRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateDiffIdCommand request, CancellationToken cancellationToken)
        {
            var diffId = new DiffId
            {
                Name = request.Name
            };

            _diffIdRepository.Add(diffId);
            await _diffIdRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return diffId.Id;
        }
    }
}