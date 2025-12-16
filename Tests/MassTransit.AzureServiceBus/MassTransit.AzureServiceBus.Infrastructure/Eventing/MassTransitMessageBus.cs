using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MassTransit;
using MassTransit.AzureServiceBus.Application.Common.Eventing;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.MassTransit.MassTransitMessageBus", Version = "1.0")]

namespace MassTransit.AzureServiceBus.Infrastructure.Eventing
{
    public class MassTransitMessageBus : IEventBus
    {
        public const string AddressKey = "address";
        private readonly List<MessageEntry> _messagesToDispatch = [];
        private readonly IServiceProvider _serviceProvider;

        public MassTransitMessageBus(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public ConsumeContext? ConsumeContext { get; set; }

        public void Publish<TMessage>(TMessage message)
            where TMessage : class
        {
            _messagesToDispatch.Add(new MessageEntry(message, null, DispatchType.Publish));
        }

        public void Send<TMessage>(TMessage message)
            where TMessage : class
        {
            _messagesToDispatch.Add(new MessageEntry(message, null, DispatchType.Send));
        }

        public void Send<TMessage>(TMessage message, Uri address)
            where TMessage : class
        {
            _messagesToDispatch.Add(new MessageEntry(message, new Dictionary<string, object>
            {
                { AddressKey, address.ToString() }
            }, DispatchType.Send));
        }

        public async Task FlushAllAsync(CancellationToken cancellationToken = default)
        {
            var messagesToPublish = _messagesToDispatch.Where(x => x.DispatchType == DispatchType.Publish).ToList();
            var messagesToSend = _messagesToDispatch.Where(x => x.DispatchType == DispatchType.Send).ToList();
            await PublishMessagesAsync(messagesToPublish, cancellationToken);
            await SendMessagesAsync(messagesToSend, cancellationToken);

            _messagesToDispatch.Clear();
        }

        private async Task PublishMessagesAsync(List<MessageEntry> messagesToPublish, CancellationToken cancellationToken)
        {
            if (ConsumeContext is not null)
            {
                await ConsumeContext!.PublishBatch(messagesToPublish.Select(x => x.Message), cancellationToken).ConfigureAwait(false);
                return;
            }
            var publishEndpoint = _serviceProvider.GetRequiredService<IPublishEndpoint>();
            await publishEndpoint.PublishBatch(messagesToPublish.Select(x => x.Message), cancellationToken).ConfigureAwait(false);
        }

        private async Task SendMessagesAsync(List<MessageEntry> messagesToSend, CancellationToken cancellationToken)
        {
            foreach (var toSend in messagesToSend)
            {
                Uri? address = null;

                if (toSend.AdditionalData?.TryGetValue(AddressKey, out var addressObj) == true)
                {
                    address = new Uri((string)addressObj);
                }

                if (ConsumeContext is not null)
                {
                    if (address is null)
                    {
                        await ConsumeContext!.Send(toSend.Message, cancellationToken).ConfigureAwait(false);
                    }
                    else
                    {
                        var endpoint = await ConsumeContext!.GetSendEndpoint(address).ConfigureAwait(false);
                        await endpoint.Send(toSend.Message, cancellationToken).ConfigureAwait(false);
                    }
                }
                else
                {
                    if (address is null)
                    {
                        var bus = _serviceProvider.GetRequiredService<IBus>();
                        await bus.Send(toSend.Message, cancellationToken).ConfigureAwait(false);
                    }
                    else
                    {
                        var sendEndpointProvider = _serviceProvider.GetRequiredService<ISendEndpointProvider>();
                        var endpoint = await sendEndpointProvider.GetSendEndpoint(address).ConfigureAwait(false);
                        await endpoint.Send(toSend.Message, cancellationToken).ConfigureAwait(false);
                    }
                }
            }
        }

        private enum DispatchType
        {
            Publish,

            Send
        }
        private sealed record MessageEntry(object Message, IDictionary<string, object>? AdditionalData, DispatchType DispatchType);

    }
}