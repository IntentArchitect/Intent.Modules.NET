using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Solace.BaseMessage", Version = "1.0")]

namespace Solace.Tests.Application
{
    public abstract record BaseMessage
    {
        public BaseMessage()
        {
            var type = this.GetType();
            MessageType = $"{type.Namespace}.{type.Name}";
        }

        public string MessageType { get; set; }
    }
}