using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MassTransitFinbuckle.Test.Application.IntegrationServices;
using MassTransitFinbuckle.Test.Application.IntegrationServices.Contracts.Services.RequestResponse;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace MassTransitFinbuckle.Test.Application.RequestResponse.Initial
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class InitialCommandHandler : IRequestHandler<InitialCommand>
    {
        private readonly IRequestResponseService _requestResponseService;

        [IntentManaged(Mode.Merge)]
        public InitialCommandHandler(IRequestResponseService requestResponseService)
        {
            _requestResponseService = requestResponseService;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Handle(InitialCommand request, CancellationToken cancellationToken)
        {
            await _requestResponseService.TestAsync(TestCommand.Create(request.Value), cancellationToken);
        }
    }
}