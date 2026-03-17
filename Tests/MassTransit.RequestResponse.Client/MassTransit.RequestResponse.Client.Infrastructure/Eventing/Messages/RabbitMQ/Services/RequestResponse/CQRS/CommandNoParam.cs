using Intent.RoslynWeaver.Attributes;
using MassTransit.RequestResponse.Client.Application.IntegrationServices.Contracts.RabbitMQ.Services.RequestResponse.CQRS;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.MassTransit.RequestResponse.RequestResponse.MapperRequestMessage", Version = "1.0")]

namespace MassTransit.RabbitMQ.Services.RequestResponse.CQRS
{
    public class CommandNoParam
    {
        public CommandNoParam()
        {
        }

        public CommandNoParam(MassTransit.RequestResponse.Client.Application.IntegrationServices.Contracts.RabbitMQ.Services.RequestResponse.CQRS.CommandNoParam dto)
        {
        }
    }
}