using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Wolverine.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.UnhandledExceptionMiddleware", Version = "1.0")]

namespace Wolverine.AspNetCore.FastEndpoints.Infrastructure.Dispatch.Middleware
{
    public class UnhandledExceptionMiddleware
    {
        private readonly bool _logRequestPayload;

        public UnhandledExceptionMiddleware(IConfiguration configuration)
        {
            _logRequestPayload = configuration.GetValue<bool?>("CqrsSettings:LogRequestPayload") ?? false;
        }

        [WolverineOnException]
        public void OnException(Exception exception, Envelope envelope, ILogger logger)
        {
            LogException(exception, envelope.Message, logger);
            throw exception;
        }

        private void LogException(Exception exception, object? request, ILogger logger)
        {
            if (exception is ValidationException)
            {
                return;
            }
            var requestName = request?.GetType().Name;

            if (_logRequestPayload)
            {
                logger.LogError(exception, "Wolverine.AspNetCore.FastEndpoints Request: Unhandled Exception for Request {Name} {@Request}", requestName, request);
            }
            else
            {
                logger.LogError(exception, "Wolverine.AspNetCore.FastEndpoints Request: Unhandled Exception for Request {Name}", requestName);
            }
        }
    }
}