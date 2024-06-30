using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SolaceSystems.Solclient.Messaging;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Solace.SolaceConsumingService", Version = "1.0")]

namespace Solace.Tests.Infrastructure.Eventing
{
    public class SolaceConsumingService : IHostedService
    {
        private readonly List<SolaceConsumer> _consumers = new List<SolaceConsumer>();
        private readonly bool _bindQueues;
        private readonly ISession _session;
        private readonly MessageRegistry _messageRegistry;
        private readonly IServiceProvider _provider;

        public SolaceConsumingService(ISession session,
            MessageRegistry messageRegistry,
            IConfiguration configuration,
            IServiceProvider provider)
        {
            _session = session;
            _messageRegistry = messageRegistry;
            _provider = provider;
            _bindQueues = configuration.GetSection("Solace:BindQueues").Get<bool?>() ?? true;
            ValidateEnvironment(session);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (_bindQueues)
            {
                foreach (var queueConfig in _messageRegistry.Queues)
                {
                    var consumer = _provider.GetRequiredService<SolaceConsumer>();
                    consumer.Start(queueConfig);
                    _consumers.Add(consumer);
                }
            }
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            foreach (var consumer in _consumers)
            {
                consumer.Stop();
            }
            return Task.CompletedTask;
        }

        private void ValidateEnvironment(ISession session)
        {
            var returnCode = session.Connect();
            if (returnCode != ReturnCode.SOLCLIENT_OK)
            {
                throw new InvalidOperationException("Error connecting to Solace, return code: {0}\", returnCode");
            }
            if (!session.IsCapable(CapabilityType.PUB_GUARANTEED))
            {
                throw new InvalidOperationException("Cannot proceed because session's PUB_GUARANTEED capability is not supported.");
            }
            if (!session.IsCapable(CapabilityType.SUB_FLOW_GUARANTEED))
            {
                throw new InvalidOperationException("Cannot proceed because session's SUB_FLOW_GUARANTEED capability is not supported.");
            }
            if (!session.IsCapable(CapabilityType.ENDPOINT_MANAGEMENT))
            {
                throw new InvalidOperationException("Cannot proceed because session's ENDPOINT_MANAGEMENT capability is not supported.");
            }
            if (!session.IsCapable(CapabilityType.QUEUE_SUBSCRIPTIONS))
            {
                throw new InvalidOperationException("Cannot proceed because session's QUEUE_SUBSCRIPTIONS capability is not supported.");
            }
        }
    }
}