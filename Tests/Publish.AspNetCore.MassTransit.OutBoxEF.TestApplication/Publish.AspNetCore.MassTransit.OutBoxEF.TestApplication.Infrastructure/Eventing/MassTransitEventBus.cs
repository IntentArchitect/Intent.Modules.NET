using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Publish.AspNetCore.MassTransit.OutBoxEF.TestApplication.Application.Common.Eventing;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.MassTransit.MassTransitEventBus", Version = "1.0")]

namespace Publish.AspNetCore.MassTransit.OutBoxEF.TestApplication.Infrastructure.Eventing
{
    public class MassTransitEventBus : IEventBus
    {
        private readonly List<object> _messagesToPublish = new List<object>();
        private readonly IServiceProvider _serviceProvider;

        public MassTransitEventBus(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public ConsumeContext? ConsumeContext { get; set; }

        public void Publish<T>(T message) where T : class
        {
            _messagesToPublish.Add(message);
        }

        public async Task FlushAllAsync(CancellationToken cancellationToken = default)
        {
            if (ConsumeContext is not null)
            {
                await PublishWithConsumeContext(cancellationToken);
            }
            else
            {
                await PublishWithNormalContext(cancellationToken);
            }

            _messagesToPublish.Clear();
        }

        private async Task PublishWithConsumeContext(CancellationToken cancellationToken)
        {
            await ConsumeContext!.PublishBatch(_messagesToPublish, cancellationToken).ConfigureAwait(false);
        }

        private async Task PublishWithNormalContext(CancellationToken cancellationToken)
        {
            var publishEndpoint = _serviceProvider.GetRequiredService<IPublishEndpoint>();

            await publishEndpoint.PublishBatch(_messagesToPublish, cancellationToken).ConfigureAwait(false);
        }
    }
}