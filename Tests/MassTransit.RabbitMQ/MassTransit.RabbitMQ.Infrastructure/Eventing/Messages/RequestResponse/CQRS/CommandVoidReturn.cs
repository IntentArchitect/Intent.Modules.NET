using Intent.RoslynWeaver.Attributes;
using MassTransit.RabbitMQ.Application.IntegrationServices.Contracts.Services.RequestResponse.CQRS;
using MassTransit.RabbitMQ.Application.RequestResponse.CQRS.CommandVoidReturn;
using MassTransit.RabbitMQ.Infrastructure.Eventing;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.MassTransit.RequestResponse.RequestResponse.MapperRequestMessage", Version = "1.0")]

namespace MassTransit.RabbitMQ.Services.RequestResponse.CQRS
{
    public class CommandVoidReturn : IMapperRequest
    {
        public CommandVoidReturn()
        {
        }

        public CommandVoidReturn(Application.IntegrationServices.Contracts.Services.RequestResponse.CQRS.CommandVoidReturn dto)
        {
            Input = dto.Input;
        }
        public string Input { get; set; }

        public object CreateRequest()
        {
            return new Application.RequestResponse.CQRS.CommandVoidReturn.CommandVoidReturn(Input);
        }
    }
}