using Intent.RoslynWeaver.Attributes;
using MassTransit.RabbitMQ.Application.IntegrationServices.Contracts.Services.RequestResponse.CQRS;
using MassTransit.RabbitMQ.Application.RequestResponse.CQRS.CommandNoParam;
using MassTransit.RabbitMQ.Infrastructure.Eventing;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.MassTransit.RequestResponse.RequestResponse.MapperRequestMessage", Version = "1.0")]

namespace MassTransit.RabbitMQ.Services.RequestResponse.CQRS
{
    public class CommandNoParam : IMapperRequest
    {
        public CommandNoParam()
        {
        }

        public CommandNoParam(Application.IntegrationServices.Contracts.Services.RequestResponse.CQRS.CommandNoParam dto)
        {
        }
        public object CreateRequest()
        {
            return new Application.RequestResponse.CQRS.CommandNoParam.CommandNoParam();
        }
    }
}