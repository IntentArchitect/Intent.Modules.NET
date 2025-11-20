using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CompositePublishTest.Application.Common.Eventing;
using Intent.RoslynWeaver.Attributes;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.MassTransit.MassTransitEventBus", Version = "1.0")]

namespace CompositePublishTest.Infrastructure.Eventing
{
    public class MassTransitMessageBus : IMessageBus
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly List<MessageEntry> _messagesToDispatch = [];

        public MassTransitMessageBus(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public ConsumeContext? ConsumeContext { get; set; }

        public void Publish<T>(T message) where T : class
        {
            _messagesToDispatch.Add(new MessageEntry(message, null, DispatchType.Publish));
        }

        public void Publish<T>(T message, IDictionary<string, object> additionalData) where T : class
        {
            _messagesToDispatch.Add(new MessageEntry(message, additionalData, DispatchType.Publish));
        }

        public void Send<T>(T message)
            where T : class
        {
            _messagesToDispatch.Add(new MessageEntry(message, null, DispatchType.Send));
        }

        public void Send<T>(T message, IDictionary<string, object> additionalData) where T : class
        {
            _messagesToDispatch.Add(new MessageEntry(message, additionalData, DispatchType.Send));
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
                await ConsumeContext!.PublishBatch(messagesToPublish, cancellationToken).ConfigureAwait(false);
                return;
            }
            
            var publishEndpoint = _serviceProvider.GetRequiredService<IPublishEndpoint>();
            await publishEndpoint.PublishBatch(messagesToPublish, cancellationToken).ConfigureAwait(false);
        }
        
        private async Task SendMessagesAsync(List<MessageEntry> messagesToSend, CancellationToken cancellationToken)
        {
            foreach (var toSend in messagesToSend)
            {
                Uri? address = null;
                if (toSend.AdditionalData?.TryGetValue("address", out var addressObj) == true)
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

        private record MessageEntry(object Message, IDictionary<string, object>? AdditionalData, DispatchType DispatchType);
    }
}