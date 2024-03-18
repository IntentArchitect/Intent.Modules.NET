using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Hosting;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Kafka.KafkaConsumerBackgroundService", Version = "1.0")]

namespace Kafka.Producer.Infrastructure.Eventing
{
    public class KafkaConsumerBackgroundService : BackgroundService
    {
        private readonly IEnumerable<IKafkaConsumer> _consumers;

        public KafkaConsumerBackgroundService(IEnumerable<IKafkaConsumer> consumers)
        {
            _consumers = consumers;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.WhenAll(_consumers.Select(x => Task.Run(() => x.DoWork(stoppingToken), stoppingToken)));
        }
    }
}