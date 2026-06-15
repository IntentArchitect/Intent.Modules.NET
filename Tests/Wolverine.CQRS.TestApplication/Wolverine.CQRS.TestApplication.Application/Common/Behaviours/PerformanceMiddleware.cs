using System.Diagnostics;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Wolverine;
using Wolverine.CQRS.TestApplication.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]

namespace Wolverine.CQRS.TestApplication.Application.Common.Behaviours
{
    public class PerformanceMiddleware
    {
        private const long LongRunningThresholdMilliseconds = 500;
        private readonly bool _logRequestPayload;

        public PerformanceMiddleware(IConfiguration configuration)
        {
            _logRequestPayload = configuration.GetValue<bool?>("CqrsSettings:LogRequestPayload") ?? false;
        }

        public Stopwatch Before(Envelope envelope)
        {
            return StartTimer();
        }

        public async Task FinallyAsync(Stopwatch stopwatch, Envelope envelope, ILogger logger, ICurrentUserService currentUserService, CancellationToken cancellationToken)
        {
            await LogIfSlowAsync(stopwatch, envelope.Message, logger, currentUserService);
        }

        private static Stopwatch StartTimer()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            return stopwatch;
        }

        private async Task LogIfSlowAsync(Stopwatch stopwatch, object request, ILogger logger, ICurrentUserService currentUserService)
        {
            stopwatch.Stop();

            if (stopwatch.ElapsedMilliseconds <= LongRunningThresholdMilliseconds)
            {
                return;
            }

            var requestName = request.GetType().Name;
            var user = await currentUserService.GetAsync();

            if (_logRequestPayload)
            {
                logger.LogWarning("Wolverine.CQRS.TestApplication Long Running Request: {Name} ({ElapsedMilliseconds} milliseconds) {@UserId} {@UserName} {@Request}", requestName, stopwatch.ElapsedMilliseconds, user?.Id, user?.Name, request);
            }
            else
            {
                logger.LogWarning("Wolverine.CQRS.TestApplication Long Running Request: {Name} ({ElapsedMilliseconds} milliseconds) {@UserId} {@UserName}", requestName, stopwatch.ElapsedMilliseconds, user?.Id, user?.Name);
            }
        }
    }
}
