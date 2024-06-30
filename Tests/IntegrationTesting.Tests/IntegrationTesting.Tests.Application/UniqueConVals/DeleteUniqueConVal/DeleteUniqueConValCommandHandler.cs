using System;
using System.Threading;
using System.Threading.Tasks;
using IntegrationTesting.Tests.Domain.Common.Exceptions;
using IntegrationTesting.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.UniqueConVals.DeleteUniqueConVal
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteUniqueConValCommandHandler : IRequestHandler<DeleteUniqueConValCommand>
    {
        private readonly IUniqueConValRepository _uniqueConValRepository;

        [IntentManaged(Mode.Merge)]
        public DeleteUniqueConValCommandHandler(IUniqueConValRepository uniqueConValRepository)
        {
            _uniqueConValRepository = uniqueConValRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(DeleteUniqueConValCommand request, CancellationToken cancellationToken)
        {
            var uniqueConVal = await _uniqueConValRepository.FindByIdAsync(request.Id, cancellationToken);
            if (uniqueConVal is null)
            {
                throw new NotFoundException($"Could not find UniqueConVal '{request.Id}'");
            }

            _uniqueConValRepository.Remove(uniqueConVal);
        }
    }
}