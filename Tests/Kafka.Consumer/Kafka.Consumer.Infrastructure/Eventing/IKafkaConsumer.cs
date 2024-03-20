using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Kafka.KafkaConsumerInterface", Version = "1.0")]

namespace Kafka.Consumer.Infrastructure.Eventing
{
    public interface IKafkaConsumer
    {
        Task DoWork(CancellationToken stoppingToken);
    }
}