using System.Diagnostics;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Wolverine.Subscribe.RabbitMQ.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.PerformanceMiddleware", Version = "1.0")]

namespace Wolverine.Subscribe.RabbitMQ.Infrastructure.Dispatch.Middleware
{
    public class PerformanceMiddleware
    {
        private readonly long _longRunningThresholdMilliseconds = 500;
        private readonly bool _logRequestPayload;

        public PerformanceMiddleware(IConfiguration configuration)
        {
            _logRequestPayload = configuration.GetValue<bool?>("CqrsSettings:LogRequestPayload") ?? false;
        }

        public Stopwatch Before(Envelope envelope)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            return stopwatch;
        }

        public async Task FinallyAsync(
            Stopwatch stopwatch,
            Envelope envelope,
            ILogger logger,
            ICurrentUserService currentUserService,
            CancellationToken cancellationToken)
        {
            stopwatch.Stop();

            if (stopwatch.ElapsedMilliseconds <= _longRunningThresholdMilliseconds)
            {
                return;
            }
            var requestName = envelope.Message?.GetType().Name;
            var user = await currentUserService.GetAsync();

            if (_logRequestPayload)
            {
                logger.LogWarning("Wolverine.Subscribe.RabbitMQ Long Running Request: {Name} ({ElapsedMilliseconds} milliseconds) {@UserId} {@UserName} {@Request}", requestName, stopwatch.ElapsedMilliseconds, user?.Id, user?.Name, envelope.Message);
            }
            else
            {
                logger.LogWarning("Wolverine.Subscribe.RabbitMQ Long Running Request: {Name} ({ElapsedMilliseconds} milliseconds) {@UserId} {@UserName}", requestName, stopwatch.ElapsedMilliseconds, user?.Id, user?.Name);
            }
        }
    }
}