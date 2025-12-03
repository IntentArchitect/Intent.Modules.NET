using CompositeMessageBus.Application.Common.Eventing;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.AzureEventGrid.EventContext", Version = "1.0")]

namespace CompositeMessageBus.Infrastructure.Eventing
{
    public class EventContext : IEventContext
    {
        public IDictionary<string, object> AdditionalData { get; } = new Dictionary<string, object>();
    }
}