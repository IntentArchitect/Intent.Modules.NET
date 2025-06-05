using System;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging;
using Microsoft.Extensions.Logging;

namespace AzureFunctions.AzureEventGrid.Infrastructure.Eventing.Behaviors
{
    public class RetryGridBehavior : IAzureEventGridBehavior
    {
        private readonly ILogger<RetryGridBehavior> _logger;
        private readonly int _maxRetries;
        private readonly TimeSpan _delay;

        public RetryGridBehavior(ILogger<RetryGridBehavior> logger, int maxRetries = 3, int delayMilliseconds = 1000)
        {
            _logger = logger;
            _maxRetries = maxRetries;
            _delay = TimeSpan.FromMilliseconds(delayMilliseconds);
        }

        public async Task<CloudEvent> HandleAsync(CloudEvent message, CloudEventBehaviorDelegate next, CancellationToken cancellationToken = default)
        {
            for (int i = 0; i <= _maxRetries; i++)
            {
                try
                {
                    return await next(message, cancellationToken);
                }
                catch (Exception ex) when (i < _maxRetries)
                {
                    _logger.LogWarning(ex, "Attempt {Attempt} of {MaxRetries} failed for message type {MessageType}",
                        i + 1, _maxRetries, message.Type);
                    
                    await Task.Delay(_delay, cancellationToken);
                }
            }

            throw new Exception($"Failed to process message of type {message.Type} after {_maxRetries} retries");
        }
    }
} 