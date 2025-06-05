using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging;
using Microsoft.Extensions.Logging;

namespace AzureFunctions.AzureEventGrid.Infrastructure.Eventing.Behaviors
{
    public class LoggingGridBehavior : IAzureEventGridBehavior
    {
        private readonly ILogger<LoggingGridBehavior> _logger;

        public LoggingGridBehavior(ILogger<LoggingGridBehavior> logger)
        {
            _logger = logger;
        }

        public async Task<CloudEvent> HandleAsync(CloudEvent message, CloudEventBehaviorDelegate next, CancellationToken cancellationToken = default)
        {
            var messageType = message.Type;
            _logger.LogInformation("Processing message of type {MessageType}", messageType);
            
            try
            {
                var result = await next(message, cancellationToken);
                _logger.LogInformation("Successfully processed message of type {MessageType}", messageType);
                return result;
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error processing message of type {MessageType}", messageType);
                throw;
            }
        }
    }
} 