using System.Diagnostics;
using CqrsSpike.Subscriber.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using Mediator;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Mediator.Behaviours.PerformanceBehaviour", Version = "1.0")]

namespace CqrsSpike.Subscriber.Application.Common.Behaviours
{
    public class PerformanceBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IMessage
    {
        private readonly ILogger<PerformanceBehaviour<TRequest, TResponse>> _logger;
        private readonly ICurrentUserService _currentUserService;
        private readonly Stopwatch _timer;
        private readonly bool _logRequestPayload;

        public PerformanceBehaviour(ILogger<PerformanceBehaviour<TRequest, TResponse>> logger,
            ICurrentUserService currentUserService,
            IConfiguration configuration)
        {
            _timer = new Stopwatch();
            _logger = logger;
            _currentUserService = currentUserService;
            _logRequestPayload = configuration.GetValue<bool?>("CqrsSettings:LogRequestPayload") ?? false;
        }

        public async ValueTask<TResponse> Handle(
            TRequest request,
            MessageHandlerDelegate<TRequest, TResponse> next,
            CancellationToken cancellationToken)
        {
            _timer.Start();

            var response = await next(request, cancellationToken);

            _timer.Stop();

            var elapsedMilliseconds = _timer.ElapsedMilliseconds;

            if (elapsedMilliseconds > 500)
            {
                var requestName = typeof(TRequest).Name;
                var user = await _currentUserService.GetAsync();

                if (_logRequestPayload)
                {
                    _logger.LogWarning("CqrsSpike.Subscriber Long Running Request: {Name} ({ElapsedMilliseconds} milliseconds) {@UserId} {@UserName} {@Request}", requestName, elapsedMilliseconds, user?.Id, user?.Name, request);
                }
                else
                {
                    _logger.LogWarning("CqrsSpike.Subscriber Long Running Request: {Name} ({ElapsedMilliseconds} milliseconds) {@UserId} {@UserName}", requestName, elapsedMilliseconds, user?.Id, user?.Name);
                }
            }
            return response;
        }
    }
}