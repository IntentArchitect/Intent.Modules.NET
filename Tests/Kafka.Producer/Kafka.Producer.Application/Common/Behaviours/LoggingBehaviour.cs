using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Kafka.Producer.Application.Common.Interfaces;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.Behaviours.LoggingBehaviour", Version = "1.0")]

namespace Kafka.Producer.Application.Common.Behaviours
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

            _logger.LogInformation("Kafka.Producer Request: {Name} {@UserId} {@UserName} {@Request}",
                requestName, userId, userName, request);
            return Task.CompletedTask;
        }
    }
}