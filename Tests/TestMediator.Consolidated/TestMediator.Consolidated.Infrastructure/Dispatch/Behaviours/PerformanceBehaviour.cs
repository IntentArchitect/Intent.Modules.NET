using System.Diagnostics;
using Intent.RoslynWeaver.Attributes;
using Mediator;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Mediator.PerformanceBehaviour", Version = "1.0")]

namespace TestMediator.Consolidated.Infrastructure.Dispatch.Behaviours
{
    public class PerformanceBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : global::Mediator.IMessage
    {
        private readonly ILogger<PerformanceBehaviour<TRequest, TResponse>> _logger;
        private readonly bool _logRequestPayload;

        public PerformanceBehaviour(ILogger<PerformanceBehaviour<TRequest, TResponse>> logger,
            IConfiguration configuration)
        {
            _logger = logger;
            _logRequestPayload = configuration.GetValue<bool?>("CqrsSettings:LogRequestPayload") ?? false;
        }

        public async ValueTask<TResponse> Handle(
            TRequest request,
            MessageHandlerDelegate<TRequest, TResponse> next,
            CancellationToken cancellationToken)
        {
            var started = Stopwatch.GetTimestamp();

            var response = await next(request, cancellationToken);

            var elapsedMilliseconds = (long)Stopwatch.GetElapsedTime(started).TotalMilliseconds;

            if (elapsedMilliseconds > 500)
            {
                var requestName = typeof(TRequest).Name;

                if (_logRequestPayload)
                {
                    _logger.LogWarning("TestMediator.Consolidated Long Running Request: {Name} ({ElapsedMilliseconds} milliseconds) {@Request}", requestName, elapsedMilliseconds, request);
                }
                else
                {
                    _logger.LogWarning("TestMediator.Consolidated Long Running Request: {Name} ({ElapsedMilliseconds} milliseconds)", requestName, elapsedMilliseconds);
                }
            }
            return response;
        }
    }
}