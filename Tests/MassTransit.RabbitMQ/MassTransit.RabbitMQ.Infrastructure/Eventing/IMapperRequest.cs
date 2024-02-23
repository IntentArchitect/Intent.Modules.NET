using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.MassTransit.RequestResponse.RequestResponse.MapperRequestInterface", Version = "1.0")]

namespace MassTransit.RabbitMQ.Infrastructure.Eventing
{
    public interface IMapperRequest
    {
        object CreateRequest();
    }
}