using Intent.RoslynWeaver.Attributes;
using MassTransit.RabbitMQ.Application.RequestResponse.CQRS.CommandGuidReturn;
using MassTransit.RabbitMQ.Infrastructure.Eventing;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.MassTransit.RequestResponse.RequestResponse.MapperRequestMessage", Version = "1.0")]

namespace MassTransit.RabbitMQ.Services.RequestResponse.CQRS
{
    public class CommandGuidReturn : IMapperRequest
    {
        public CommandGuidReturn()
        {
        }
        public string Input { get; set; }

        public object CreateRequest()
        {
            return new Application.RequestResponse.CQRS.CommandGuidReturn.CommandGuidReturn(Input);
        }
    }
}