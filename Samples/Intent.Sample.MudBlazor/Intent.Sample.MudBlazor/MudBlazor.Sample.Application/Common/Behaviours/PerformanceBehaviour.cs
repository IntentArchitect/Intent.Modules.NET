using System.Diagnostics;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MudBlazor.Sample.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.Behaviours.PerformanceBehaviour", Version = "1.0")]

namespace MudBlazor.Sample.Application.Common.Behaviours
{
    public class PerformanceBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
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

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            _timer.Start();

            var response = await next(cancellationToken);

            _timer.Stop();

            var elapsedMilliseconds = _timer.ElapsedMilliseconds;

            if (elapsedMilliseconds > 500)
            {
                var requestName = typeof(TRequest).Name;
                var userId = _currentUserService.UserId;
                var userName = _currentUserService.UserName;

                if (_logRequestPayload)
                {
                    _logger.LogWarning("Intent.Sample.MudBlazor Long Running Request: {Name} ({ElapsedMilliseconds} milliseconds) {@UserId} {@UserName} {@Request}", requestName, elapsedMilliseconds, userId, userName, request);
                }
                else
                {
                    _logger.LogWarning("Intent.Sample.MudBlazor Long Running Request: {Name} ({ElapsedMilliseconds} milliseconds) {@UserId} {@UserName}", requestName, elapsedMilliseconds, userId, userName);
                }
            }
            return response;
        }
    }
}