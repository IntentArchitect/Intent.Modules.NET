using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Google.Cloud.PubSub.V1;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Publish.AspNetCore.GooglePubSub.TestApplication.Application.Common.Eventing;
using Publish.AspNetCore.GooglePubSub.TestApplication.Application.IntegrationEvents;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.GoogleCloud.PubSub.EventingTemplates.GoogleEventBusSubscriptionManager", Version = "1.0")]

namespace Publish.AspNetCore.GooglePubSub.TestApplication.Infrastructure.Eventing;

public class GoogleEventBusSubscriptionManager : IEventBusSubscriptionManager
{
    private delegate Task MessageHandlerFunc(IServiceProvider provider, PubsubMessage message, CancellationToken cancellationToken);

    private readonly Dictionary<string, MessageHandlerFunc> _handlers;

    public GoogleEventBusSubscriptionManager()
    {
        _handlers = new Dictionary<string, MessageHandlerFunc>();
    }

    public void RegisterEventHandler<TMessage>()
        where TMessage : class
    {
        _handlers.Add(typeof(TMessage).FullName!, async (provider, message, cancellationToken) =>
        {
            var messageObj = JsonSerializer.Deserialize<TMessage>(System.Text.Encoding.UTF8.GetString(message.Data.ToArray()));
            var handler = provider.GetService<IIntegrationEventHandler<TMessage>>()!;
            await handler.HandleAsync(messageObj!, cancellationToken);
        });
    }

    public async Task DispatchAsync(IServiceProvider serviceProvider, PubsubMessage message, CancellationToken cancellationToken)
    {
        if (!message.Attributes.TryGetValue("MessageType", out var messageTypeStr) ||
            string.IsNullOrEmpty(messageTypeStr) ||
            !_handlers.TryGetValue(messageTypeStr, out var messageHandler))
        {
            var handler = serviceProvider.GetService<IIntegrationEventHandler<GenericMessage>>()!;
            await handler.HandleAsync(new GenericMessage(
                MessageId: message.MessageId,
                Attributes: message.Attributes,
                MessageBody: System.Text.Encoding.UTF8.GetString(message.Data.ToArray())), cancellationToken);
            return;
        }

        await messageHandler(serviceProvider, message, cancellationToken);
    }
}