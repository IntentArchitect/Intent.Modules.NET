using System;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Application.AsyncRepository.Operation
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class OperationCommandHandler : IRequestHandler<OperationCommand>
    {
        private readonly IAsyncRepositoryTestRepository _asyncRepositoryTestRepository;

        [IntentManaged(Mode.Merge)]
        public OperationCommandHandler(IAsyncRepositoryTestRepository asyncRepositoryTestRepository)
        {
            _asyncRepositoryTestRepository = asyncRepositoryTestRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(OperationCommand request, CancellationToken cancellationToken)
        {
            await _asyncRepositoryTestRepository.Operation(cancellationToken);
        }
    }
}