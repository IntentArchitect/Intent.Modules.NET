using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.PerformanceMiddleware", Version = "1.0")]

namespace Wolverine.AwsLambdaFunctions.Infrastructure.Dispatch.Middleware
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
            CancellationToken cancellationToken)
        {
            stopwatch.Stop();

            if (stopwatch.ElapsedMilliseconds <= _longRunningThresholdMilliseconds)
            {
                return;
            }
            var requestName = envelope.Message?.GetType().Name;

            if (_logRequestPayload)
            {
                logger.LogWarning("Wolverine.AwsLambdaFunctions Long Running Request: {Name} ({ElapsedMilliseconds} milliseconds) {@Request}", requestName, stopwatch.ElapsedMilliseconds, envelope.Message);
            }
            else
            {
                logger.LogWarning("Wolverine.AwsLambdaFunctions Long Running Request: {Name} ({ElapsedMilliseconds} milliseconds)", requestName, stopwatch.ElapsedMilliseconds);
            }
        }
    }
}