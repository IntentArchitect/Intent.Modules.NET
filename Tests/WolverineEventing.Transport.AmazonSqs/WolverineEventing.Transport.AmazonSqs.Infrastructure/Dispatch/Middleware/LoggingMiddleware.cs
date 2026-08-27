using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Wolverine;
using WolverineEventing.Transport.AmazonSqs.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.LoggingMiddleware", Version = "1.0")]

namespace WolverineEventing.Transport.AmazonSqs.Infrastructure.Dispatch.Middleware
{
    public class LoggingMiddleware
    {
        private readonly bool _logRequestPayload;

        public LoggingMiddleware(IConfiguration configuration)
        {
            _logRequestPayload = configuration.GetValue<bool?>("CqrsSettings:LogRequestPayload") ?? false;
        }

        public async Task BeforeAsync(
            Envelope envelope,
            ILogger logger,
            ICurrentUserService currentUserService,
            CancellationToken cancellationToken)
        {
            if (envelope.Message is null)
            {
                return;
            }
            await LogAsync(envelope.Message, logger, currentUserService);
        }

        private async Task LogAsync(object request, ILogger logger, ICurrentUserService currentUserService)
        {
            var requestName = request.GetType().Name;
            var user = await currentUserService.GetAsync();

            if (_logRequestPayload)
            {
                logger.LogInformation("WolverineEventing.Transport.AmazonSqs Request: {Name} {@UserId} {@UserName} {@Request}", requestName, user?.Id, user?.Name, request);
            }
            else
            {
                logger.LogInformation("WolverineEventing.Transport.AmazonSqs Request: {Name} {@UserId} {@UserName}", requestName, user?.Id, user?.Name);
            }
        }
    }
}