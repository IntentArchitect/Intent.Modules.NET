using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.MassTransit.RequestResponse.RequestResponse.RequestCompletedMessage", Version = "1.0")]

namespace MassTransitFinbuckle.Test.Services.RequestResponse
{
    public class RequestCompletedMessage
    {
        public static RequestCompletedMessage Instance { get; } = new RequestCompletedMessage();
    }

    public class RequestCompletedMessage<T>
    {
        public RequestCompletedMessage()
        {
            Payload = default!;
        }

        public RequestCompletedMessage(T payload)
        {
            Payload = payload;
        }

        public T Payload { get; set; }
    }
}