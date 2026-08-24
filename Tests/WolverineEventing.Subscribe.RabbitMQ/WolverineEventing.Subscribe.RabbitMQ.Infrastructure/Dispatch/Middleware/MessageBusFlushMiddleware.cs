using Intent.RoslynWeaver.Attributes;
using WolverineEventing.Subscribe.RabbitMQ.Application.Common.Eventing;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.MessageBusFlushMiddleware", Version = "1.0")]

namespace WolverineEventing.Subscribe.RabbitMQ.Infrastructure.Dispatch.Middleware
{
    public class MessageBusFlushMiddleware
    {
        public static async Task AfterAsync(IMessageBus messageBus, CancellationToken cancellationToken)
        {
            await messageBus.FlushAllAsync(cancellationToken);
        }
    }
}