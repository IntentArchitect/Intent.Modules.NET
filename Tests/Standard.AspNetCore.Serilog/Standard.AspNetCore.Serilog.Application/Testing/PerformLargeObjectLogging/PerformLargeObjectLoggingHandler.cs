using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.Extensions.Logging;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Standard.AspNetCore.Serilog.Application.Testing.PerformLargeObjectLogging
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class PerformLargeObjectLoggingHandler : IRequestHandler<PerformLargeObjectLogging>
    {
        private readonly ILogger<PerformLargeObjectLoggingHandler> _logger;

        [IntentManaged(Mode.Merge)]
        public PerformLargeObjectLoggingHandler(ILogger<PerformLargeObjectLoggingHandler> logger)
        {
            _logger = logger;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Handle(PerformLargeObjectLogging request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Large Object Logging: {Request}", new ComplexTypeTest());
        }
    }
}