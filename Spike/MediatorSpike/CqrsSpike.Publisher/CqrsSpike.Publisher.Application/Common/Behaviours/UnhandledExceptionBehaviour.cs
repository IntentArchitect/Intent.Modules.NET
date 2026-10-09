using FluentValidation;
using Mediator;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CqrsSpike.Publisher.Application.Common.Behaviours
{
    public class UnhandledExceptionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IMessage
    {
        private readonly ILogger<UnhandledExceptionBehaviour<TRequest, TResponse>> _logger;
        private readonly bool _logRequestPayload;

        public UnhandledExceptionBehaviour(ILogger<UnhandledExceptionBehaviour<TRequest, TResponse>> logger,
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
            try
            {
                return await next(request, cancellationToken);
            }
            catch (ValidationException)
            {
                // Do not log Fluent Validation Exceptions
                throw;
            }
            catch (Exception ex)
            {
                var requestName = typeof(TRequest).Name;

                if (_logRequestPayload)
                {
                    _logger.LogError(ex, "CqrsSpike.Publisher Request: Unhandled Exception for Request {Name} {@Request}", requestName, request);
                }
                else
                {
                    _logger.LogError(ex, "CqrsSpike.Publisher Request: Unhandled Exception for Request {Name}", requestName);
                }
                throw;
            }
        }
    }
}