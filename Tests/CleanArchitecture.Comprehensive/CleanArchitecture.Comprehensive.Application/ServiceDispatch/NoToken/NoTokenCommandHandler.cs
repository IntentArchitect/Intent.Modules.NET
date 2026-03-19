using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Application.Interfaces.ServiceDispatch;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.ServiceDispatch.NoToken
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class NoTokenCommandHandler : IRequestHandler<NoTokenCommand>
    {
        private readonly IServiceDispatchService _serviceDispatchService;

        [IntentManaged(Mode.Merge)]
        public NoTokenCommandHandler(IServiceDispatchService serviceDispatchService)
        {
            _serviceDispatchService = serviceDispatchService;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(NoTokenCommand request, CancellationToken cancellationToken)
        {
            await _serviceDispatchService.Query9NoToken(request.Input);
        }
    }
}