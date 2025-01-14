using System;
using System.Threading;
using System.Threading.Tasks;
using IntegrationTesting.Tests.Domain.Common.Exceptions;
using IntegrationTesting.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.CheckNewCompChildCruds.DeleteCheckNewCompChildCrud
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteCheckNewCompChildCrudCommandHandler : IRequestHandler<DeleteCheckNewCompChildCrudCommand>
    {
        private readonly ICheckNewCompChildCrudRepository _checkNewCompChildCrudRepository;

        [IntentManaged(Mode.Merge)]
        public DeleteCheckNewCompChildCrudCommandHandler(ICheckNewCompChildCrudRepository checkNewCompChildCrudRepository)
        {
            _checkNewCompChildCrudRepository = checkNewCompChildCrudRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(DeleteCheckNewCompChildCrudCommand request, CancellationToken cancellationToken)
        {
            var checkNewCompChildCrud = await _checkNewCompChildCrudRepository.FindByIdAsync(request.Id, cancellationToken);
            if (checkNewCompChildCrud is null)
            {
                throw new NotFoundException($"Could not find CheckNewCompChildCrud '{request.Id}'");
            }

            _checkNewCompChildCrudRepository.Remove(checkNewCompChildCrud);
        }
    }
}