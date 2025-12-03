using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.AzureEventGrid.EventContextInterface", Version = "1.0")]

namespace CompositeMessageBus.Application.Common.Eventing
{
    public interface IEventContext
    {
        IDictionary<string, object> AdditionalData { get; }
    }
}