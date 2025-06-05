using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging;

namespace AzureFunctions.AzureEventGrid.Infrastructure.Eventing.Behaviors;

public class AzureEventGridPipeline
{
    private readonly List<IAzureEventGridBehavior> _behaviors;

    public AzureEventGridPipeline(IEnumerable<IAzureEventGridBehavior> behaviors)
    {
        _behaviors = new List<IAzureEventGridBehavior>(behaviors);
    }

    public Task<CloudEvent> ExecuteAsync(CloudEvent message, Func<CloudEvent, CancellationToken, Task<CloudEvent>> finalHandler, CancellationToken cancellationToken = default)
    {
        var pipeline = BuildPipeline(finalHandler);
        return pipeline(message, cancellationToken);
    }

    private CloudEventBehaviorDelegate BuildPipeline(Func<CloudEvent, CancellationToken, Task<CloudEvent>> finalHandler)
    {
        CloudEventBehaviorDelegate pipeline = (message, token) => finalHandler(message, token);

        for (var i = _behaviors.Count - 1; i >= 0; i--)
        {
            var behavior = _behaviors[i];
            var next = pipeline;
            pipeline = (message, cancellationToken) => behavior.HandleAsync(message, next, cancellationToken);
        }

        return pipeline;
    }
}