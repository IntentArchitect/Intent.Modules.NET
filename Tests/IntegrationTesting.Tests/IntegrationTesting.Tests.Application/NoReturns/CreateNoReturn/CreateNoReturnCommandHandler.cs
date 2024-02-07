using System;
using System.Threading;
using System.Threading.Tasks;
using IntegrationTesting.Tests.Domain.Entities;
using IntegrationTesting.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.NoReturns.CreateNoReturn
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateNoReturnCommandHandler : IRequestHandler<CreateNoReturnCommand>
    {
        private readonly INoReturnRepository _noReturnRepository;

        [IntentManaged(Mode.Merge)]
        public CreateNoReturnCommandHandler(INoReturnRepository noReturnRepository)
        {
            _noReturnRepository = noReturnRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(CreateNoReturnCommand request, CancellationToken cancellationToken)
        {
            var noReturn = new NoReturn
            {
                Name = request.Name
            };

            _noReturnRepository.Add(noReturn);
        }
    }
}