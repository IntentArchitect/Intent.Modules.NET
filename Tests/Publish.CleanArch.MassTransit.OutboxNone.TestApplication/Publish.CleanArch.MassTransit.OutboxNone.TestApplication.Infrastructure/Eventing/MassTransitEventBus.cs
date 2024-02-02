using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Application.Common.Eventing;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.MassTransit.MassTransitEventBus", Version = "1.0")]

namespace Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Infrastructure.Eventing
{
    public class MassTransitEventBus : IEventBus
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly List<object> _messagesToPublish = new List<object>();
        private readonly List<MessageToSend> _messagesToSend = new List<MessageToSend>();
        private readonly List<ScheduleEntry> _messagesToSchedule = new List<ScheduleEntry>();

        public MassTransitEventBus(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public ConsumeContext? ConsumeContext { get; set; }

        public void Publish<T>(T message) where T : class
        {
            _messagesToPublish.Add(message);
        }

        public void Send<T>(T message)
            where T : class
        {
            _messagesToSend.Add(new MessageToSend(message, null));
        }

        public void Send<T>(T message, Uri address)
            where T : class
        {
            _messagesToSend.Add(new MessageToSend(message, address));
        }

        public void SchedulePublish<T>(T message, DateTime scheduled)
            where T : class
        {
            _messagesToSchedule.Add(ScheduleEntry.ForScheduled(message, scheduled));
        }

        public void SchedulePublish<T>(T message, TimeSpan delay)
            where T : class
        {
            _messagesToSchedule.Add(ScheduleEntry.ForDelay(message, delay));
        }

        private class MessageToSend
        {
            public MessageToSend(object message, Uri? address)
            {
                Message = message;
                Address = address;
            }

            public object Message { get; }
            public Uri? Address { get; }
        }

        public async Task FlushAllAsync(CancellationToken cancellationToken = default)
        {
            foreach (var toSend in _messagesToSend)
            {
                if (ConsumeContext is not null)
                {
                    await SendWithConsumeContext(toSend, cancellationToken);
                }
                else
                {
                    await SendWithNormalContext(toSend, cancellationToken);
                }
            }

            _messagesToSend.Clear();

            if (ConsumeContext is not null)
            {
                await PublishWithConsumeContext(cancellationToken);
            }
            else
            {
                await PublishWithNormalContext(cancellationToken);
            }

            _messagesToPublish.Clear();
            _messagesToSchedule.Clear();
        }

        private async Task SendWithConsumeContext(MessageToSend toSend, CancellationToken cancellationToken)
        {
            if (toSend.Address is null)
            {
                await ConsumeContext!.Send(toSend.Message, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                var endpoint = await ConsumeContext!.GetSendEndpoint(toSend.Address).ConfigureAwait(false);
                await endpoint.Send(toSend.Message, cancellationToken).ConfigureAwait(false);
            }
        }

        private async Task SendWithNormalContext(MessageToSend toSend, CancellationToken cancellationToken)
        {
            if (toSend.Address is null)
            {
                var bus = _serviceProvider.GetRequiredService<IBus>();
                await bus.Send(toSend.Message, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                var sendEndpointProvider = _serviceProvider.GetRequiredService<ISendEndpointProvider>();
                var endpoint = await sendEndpointProvider.GetSendEndpoint(toSend.Address).ConfigureAwait(false);
                await endpoint.Send(toSend.Message, cancellationToken).ConfigureAwait(false);
            }
        }

        private async Task PublishWithConsumeContext(CancellationToken cancellationToken)
        {
            await ConsumeContext!.PublishBatch(_messagesToPublish, cancellationToken).ConfigureAwait(false);

            foreach (var scheduleEntry in _messagesToSchedule)
            {
                await ConsumeContext!.SchedulePublish(scheduleEntry.Scheduled, scheduleEntry.Message, cancellationToken).ConfigureAwait(false);
            }
        }

        private async Task PublishWithNormalContext(CancellationToken cancellationToken)
        {
            var publishEndpoint = _serviceProvider.GetRequiredService<IPublishEndpoint>();

            await publishEndpoint.PublishBatch(_messagesToPublish, cancellationToken).ConfigureAwait(false);

            var messageScheduler = _serviceProvider.GetRequiredService<IMessageScheduler>();

            foreach (var scheduleEntry in _messagesToSchedule)
            {
                await messageScheduler.SchedulePublish(scheduleEntry.Scheduled, scheduleEntry.Message, cancellationToken).ConfigureAwait(false);
            }
        }

        private class ScheduleEntry
        {
            private ScheduleEntry(object message, DateTime scheduled)
            {
                Message = message;
                Scheduled = scheduled;
            }

            public object Message { get; }
            public DateTime Scheduled { get; }

            public static ScheduleEntry ForScheduled(object message, DateTime scheduled)
            {
                return new ScheduleEntry(message, scheduled);
            }

            public static ScheduleEntry ForDelay(object message, TimeSpan delay)
            {
                return new ScheduleEntry(message, DateTime.UtcNow.Add(delay));
            }
        }
    }
}