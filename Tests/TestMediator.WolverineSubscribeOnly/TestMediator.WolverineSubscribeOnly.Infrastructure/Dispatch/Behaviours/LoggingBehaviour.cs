using Intent.RoslynWeaver.Attributes;
using Mediator;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Mediator.LoggingBehaviour", Version = "1.0")]

namespace TestMediator.WolverineSubscribeOnly.Infrastructure.Dispatch.Behaviours
{
    public class LoggingBehaviour<TRequest, TResponse> : MessagePreProcessor<TRequest, TResponse>
        where TRequest : global::Mediator.IMessage
    {
        private readonly ILogger<LoggingBehaviour<TRequest, TResponse>> _logger;
        private readonly bool _logRequestPayload;

        public LoggingBehaviour(ILogger<LoggingBehaviour<TRequest, TResponse>> logger, IConfiguration configuration)
        {
            _logger = logger;
            _logRequestPayload = configuration.GetValue<bool?>("CqrsSettings:LogRequestPayload") ?? false;
        }

        protected override ValueTask Handle(TRequest request, CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;

            if (_logRequestPayload)
            {
                _logger.LogInformation("TestMediator.WolverineSubscribeOnly Request: {Name} {@Request}", requestName, request);
            }
            else
            {
                _logger.LogInformation("TestMediator.WolverineSubscribeOnly Request: {Name}", requestName);
            }

            return ValueTask.CompletedTask;
        }
    }
}