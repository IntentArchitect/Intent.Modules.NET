using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;
using OpenTelemetry.AzureAppInsights.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.Behaviours.LoggingBehaviour", Version = "1.0")]

namespace OpenTelemetry.AzureAppInsights.Application.Common.Behaviours
{
    public class LoggingBehaviour<TRequest> : IRequestPreProcessor<TRequest>
        where TRequest : notnull
    {
        private readonly ILogger _logger;
        private readonly ICurrentUserService _currentUserService;

        public LoggingBehaviour(ILogger<TRequest> logger, ICurrentUserService currentUserService)
        {
            _logger = logger;
            _currentUserService = currentUserService;
        }

        public Task Process(TRequest request, CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;
            var userId = _currentUserService.UserId ?? string.Empty;
            var userName = _currentUserService.UserName ?? string.Empty;

            _logger.LogInformation("OpenTelemetry.AzureAppInsights Request: {Name} {@UserId} {@UserName} {@Request}",
                requestName, userId, userName, request);
            return Task.CompletedTask;
        }
    }
}