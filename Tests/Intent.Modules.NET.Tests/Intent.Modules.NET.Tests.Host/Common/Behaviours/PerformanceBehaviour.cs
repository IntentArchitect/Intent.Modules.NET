using System.Diagnostics;
using Intent.Modules.NET.Tests.Application.Core.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.Behaviours.PerformanceBehaviour", Version = "1.0")]

namespace Intent.Modules.NET.Tests.Host.Common.Behaviours
{
    public class PerformanceBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly ILogger<PerformanceBehaviour<TRequest, TResponse>> _logger;
        private readonly ICurrentUserService _currentUserService;
        private readonly Stopwatch _timer;

        public PerformanceBehaviour(ILogger<PerformanceBehaviour<TRequest, TResponse>> logger,
            ICurrentUserService currentUserService)
        {
            _timer = new Stopwatch();
            _logger = logger;
            _currentUserService = currentUserService;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            _timer.Start();

            var response = await next();

            _timer.Stop();

            var elapsedMilliseconds = _timer.ElapsedMilliseconds;

            if (elapsedMilliseconds > 500)
            {
                var requestName = typeof(TRequest).Name;
                var userId = _currentUserService.UserId;
                var userName = _currentUserService.UserName;

                _logger.LogWarning("ModularMonolithAppTests Long Running Request: {Name} ({ElapsedMilliseconds} milliseconds) {@UserId} {@UserName} {@Request}", requestName, elapsedMilliseconds, userId, userName, request);
            }
            return response;
        }
    }
}