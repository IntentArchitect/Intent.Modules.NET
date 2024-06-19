using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Solace.MessageConfiguration", Version = "1.0")]

namespace Solace.Tests.Infrastructure.Eventing
{
    public class MessageConfiguration
    {
        public MessageConfiguration(Type messageType, string publishedDestination, string? subscribeDestination = null)
        {
            MessageType = messageType;
            PublishedDestination = publishedDestination;
            SubscribeDestination = subscribeDestination;
            Name = GetName(messageType);
        }

        public Type MessageType { get; set; }
        public string PublishedDestination { get; set; }
        public string? SubscribeDestination { get; set; }
        public string Name { get; set; }

        public static MessageConfiguration Create<TMessage>(
            string publishedDestination,
            string? subscribeDestination = null)
        {
            return new MessageConfiguration(typeof(TMessage), publishedDestination, subscribeDestination);
        }

        private string GetName(Type type)
        {
            return $"{type.Namespace}.{type.Name}";
        }
    }
}