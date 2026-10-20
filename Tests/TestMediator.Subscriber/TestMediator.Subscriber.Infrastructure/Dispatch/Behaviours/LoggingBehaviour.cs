using Intent.RoslynWeaver.Attributes;
using Mediator;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TestMediator.Subscriber.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Mediator.LoggingBehaviour", Version = "1.0")]

namespace TestMediator.Subscriber.Infrastructure.Dispatch.Behaviours
{
    public class LoggingBehaviour<TRequest, TResponse> : MessagePreProcessor<TRequest, TResponse>
        where TRequest : global::Mediator.IMessage
    {
        private readonly ILogger<LoggingBehaviour<TRequest, TResponse>> _logger;
        private readonly ICurrentUserService _currentUserService;
        private readonly bool _logRequestPayload;

        public LoggingBehaviour(ILogger<LoggingBehaviour<TRequest, TResponse>> logger,
            ICurrentUserService currentUserService,
            IConfiguration configuration)
        {
            _logger = logger;
            _currentUserService = currentUserService;
            _logRequestPayload = configuration.GetValue<bool?>("CqrsSettings:LogRequestPayload") ?? false;
        }

        protected override async ValueTask Handle(TRequest request, CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;
            var user = await _currentUserService.GetAsync();

            if (_logRequestPayload)
            {
                _logger.LogInformation("TestMediator.Subscriber Request: {Name} {@UserId} {@UserName} {@Request}", requestName, user?.Id, user?.Name, request);
            }
            else
            {
                _logger.LogInformation("TestMediator.Subscriber Request: {Name} {@UserId} {@UserName}", requestName, user?.Id, user?.Name);
            }
        }
    }
}