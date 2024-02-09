using System;
using System.Threading;
using System.Threading.Tasks;
using IntegrationTesting.Tests.Domain.Common.Exceptions;
using IntegrationTesting.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.NoReturns.UpdateNoReturn
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateNoReturnCommandHandler : IRequestHandler<UpdateNoReturnCommand>
    {
        private readonly INoReturnRepository _noReturnRepository;

        [IntentManaged(Mode.Merge)]
        public UpdateNoReturnCommandHandler(INoReturnRepository noReturnRepository)
        {
            _noReturnRepository = noReturnRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateNoReturnCommand request, CancellationToken cancellationToken)
        {
            var noReturn = await _noReturnRepository.FindByIdAsync(request.Id, cancellationToken);
            if (noReturn is null)
            {
                throw new NotFoundException($"Could not find NoReturn '{request.Id}'");
            }

            noReturn.Name = request.Name;
        }
    }
}