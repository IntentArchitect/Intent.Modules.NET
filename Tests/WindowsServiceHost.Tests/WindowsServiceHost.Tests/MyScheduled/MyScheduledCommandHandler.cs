using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.Extensions.Logging;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace WindowsServiceHost.Tests.MyScheduled
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class MyScheduledCommandHandler : IRequestHandler<MyScheduledCommand>
    {
        private readonly ILogger<MyScheduledCommandHandler> _logger;

        [IntentManaged(Mode.Merge)]
        public MyScheduledCommandHandler(ILogger<MyScheduledCommandHandler> logger)
        {
            _logger = logger;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Handle(MyScheduledCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("My Schedule");
        }
    }
}