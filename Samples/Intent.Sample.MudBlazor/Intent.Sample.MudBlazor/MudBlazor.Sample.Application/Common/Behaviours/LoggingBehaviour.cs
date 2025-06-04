using Intent.RoslynWeaver.Attributes;
using MediatR.Pipeline;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MudBlazor.Sample.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.Behaviours.LoggingBehaviour", Version = "1.0")]

namespace MudBlazor.Sample.Application.Common.Behaviours
{
    public class LoggingBehaviour<TRequest> : IRequestPreProcessor<TRequest>
        where TRequest : notnull
    {
        private readonly ILogger<LoggingBehaviour<TRequest>> _logger;
        private readonly ICurrentUserService _currentUserService;
        private readonly bool _logRequestPayload;

        public LoggingBehaviour(ILogger<LoggingBehaviour<TRequest>> logger,
            ICurrentUserService currentUserService,
            IConfiguration configuration)
        {
            _logger = logger;
            _currentUserService = currentUserService;
            _logRequestPayload = configuration.GetValue<bool?>("CqrsSettings:LogRequestPayload") ?? false;
        }

        public Task Process(TRequest request, CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;
            var userId = _currentUserService.UserId;
            var userName = _currentUserService.UserName;

            if (_logRequestPayload)
            {
                _logger.LogInformation("Intent.Sample.MudBlazor Request: {Name} {@UserId} {@UserName} {@Request}", requestName, userId, userName, request);
            }
            else
            {
                _logger.LogInformation("Intent.Sample.MudBlazor Request: {Name} {@UserId} {@UserName}", requestName, userId, userName);
            }
            return Task.CompletedTask;
        }
    }
}