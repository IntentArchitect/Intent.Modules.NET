using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Wolverine.AspNetCore.Controllers.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.LoggingMiddleware", Version = "1.0")]

namespace Wolverine.AspNetCore.Controllers.Infrastructure.Dispatch.Middleware
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
            await LogAsync(envelope.Message, logger, currentUserService);
        }

        private async Task LogAsync(object request, ILogger logger, ICurrentUserService currentUserService)
        {
            var requestName = request.GetType().Name;
            var user = await currentUserService.GetAsync();

            if (_logRequestPayload)
            {
                logger.LogInformation("Wolverine.AspNetCore.Controllers Request: {Name} {@UserId} {@UserName} {@Request}", requestName, user?.Id, user?.Name, request);
            }
            else
            {
                logger.LogInformation("Wolverine.AspNetCore.Controllers Request: {Name} {@UserId} {@UserName}", requestName, user?.Id, user?.Name);
            }
        }
    }
}