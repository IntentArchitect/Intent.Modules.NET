using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using ValueObjects.Record.Domain.Common.Exceptions;
using ValueObjects.Record.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace ValueObjects.Record.Application.TestEntities.DeleteTestEntity
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteTestEntityCommandHandler : IRequestHandler<DeleteTestEntityCommand>
    {
        private readonly ITestEntityRepository _testEntityRepository;

        [IntentManaged(Mode.Merge)]
        public DeleteTestEntityCommandHandler(ITestEntityRepository testEntityRepository)
        {
            _testEntityRepository = testEntityRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(DeleteTestEntityCommand request, CancellationToken cancellationToken)
        {
            var testEntity = await _testEntityRepository.FindByIdAsync(request.Id, cancellationToken);
            if (testEntity is null)
            {
                throw new NotFoundException($"Could not find TestEntity '{request.Id}'");
            }

            _testEntityRepository.Remove(testEntity);
        }
    }
}