using Intent.RoslynWeaver.Attributes;
using WolverineEventing.Coexist.Cqrs.Application.Common.Eventing;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.MessageBusFlushMiddleware", Version = "1.0")]

namespace WolverineEventing.Coexist.Cqrs.Infrastructure.Dispatch.Middleware
{
    public class MessageBusFlushMiddleware
    {
        public static async Task AfterAsync(IMessageBus messageBus, CancellationToken cancellationToken)
        {
            await messageBus.FlushAllAsync(cancellationToken);
        }
    }
}