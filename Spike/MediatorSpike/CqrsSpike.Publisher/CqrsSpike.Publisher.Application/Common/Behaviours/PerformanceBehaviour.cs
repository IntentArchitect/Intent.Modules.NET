using System.Diagnostics;
using CqrsSpike.Publisher.Application.Common.Interfaces;
using Mediator;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CqrsSpike.Publisher.Application.Common.Behaviours
{
    public class PerformanceBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IMessage
    {
        private readonly ILogger<PerformanceBehaviour<TRequest, TResponse>> _logger;
        private readonly ICurrentUserService _currentUserService;
        private readonly bool _logRequestPayload;

        public PerformanceBehaviour(ILogger<PerformanceBehaviour<TRequest, TResponse>> logger,
            ICurrentUserService currentUserService,
            IConfiguration configuration)
        {
            _logger = logger;
            _currentUserService = currentUserService;
            _logRequestPayload = configuration.GetValue<bool?>("CqrsSettings:LogRequestPayload") ?? false;
        }

        public async ValueTask<TResponse> Handle(
            TRequest request,
            MessageHandlerDelegate<TRequest, TResponse> next,
            CancellationToken cancellationToken)
        {
            // Behaviours are Scoped (one instance per message type per scope), so timing state must
            // stay local to the call rather than live in a field.
            var started = Stopwatch.GetTimestamp();

            var response = await next(request, cancellationToken);

            var elapsedMilliseconds = (long)Stopwatch.GetElapsedTime(started).TotalMilliseconds;

            if (elapsedMilliseconds > 500)
            {
                var requestName = typeof(TRequest).Name;
                var user = await _currentUserService.GetAsync();

                if (_logRequestPayload)
                {
                    _logger.LogWarning("CqrsSpike.Publisher Long Running Request: {Name} ({ElapsedMilliseconds} milliseconds) {@UserId} {@UserName} {@Request}", requestName, elapsedMilliseconds, user?.Id, user?.Name, request);
                }
                else
                {
                    _logger.LogWarning("CqrsSpike.Publisher Long Running Request: {Name} ({ElapsedMilliseconds} milliseconds) {@UserId} {@UserName}", requestName, elapsedMilliseconds, user?.Id, user?.Name);
                }
            }
            return response;
        }
    }
}