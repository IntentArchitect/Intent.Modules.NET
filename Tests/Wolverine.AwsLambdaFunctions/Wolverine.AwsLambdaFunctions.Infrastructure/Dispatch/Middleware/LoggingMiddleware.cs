using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.LoggingMiddleware", Version = "1.0")]

namespace Wolverine.AwsLambdaFunctions.Infrastructure.Dispatch.Middleware
{
    public class LoggingMiddleware
    {
        private readonly bool _logRequestPayload;

        public LoggingMiddleware(IConfiguration configuration)
        {
            _logRequestPayload = configuration.GetValue<bool?>("CqrsSettings:LogRequestPayload") ?? false;
        }

        public async Task BeforeAsync(Envelope envelope, ILogger logger, CancellationToken cancellationToken)
        {
            await LogAsync(envelope.Message, logger);
        }

        private async Task LogAsync(object request, ILogger logger)
        {
            var requestName = request.GetType().Name;

            if (_logRequestPayload)
            {
                logger.LogInformation("Wolverine.AwsLambdaFunctions Request: {Name} {@Request}", requestName, request);
            }
            else
            {
                logger.LogInformation("Wolverine.AwsLambdaFunctions Request: {Name}", requestName);
            }
        }
    }
}