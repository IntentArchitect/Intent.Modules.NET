using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.Extensions.Logging;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Standard.AspNetCore.Serilog.Application.Testing.PerformSmallObjectLogging
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class PerformSmallObjectLoggingHandler : IRequestHandler<PerformSmallObjectLogging>
    {
        private readonly ILogger<PerformSmallObjectLogging> _logger;

        [IntentManaged(Mode.Merge)]
        public PerformSmallObjectLoggingHandler(ILogger<PerformSmallObjectLogging> logger)
        {
            _logger = logger;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task Handle(PerformSmallObjectLogging request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Small Object Logging: {Request}", new DeepCollectionsObject());
        }
    }
}