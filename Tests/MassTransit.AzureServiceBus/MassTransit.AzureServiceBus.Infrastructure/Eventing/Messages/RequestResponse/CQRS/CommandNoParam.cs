using Intent.RoslynWeaver.Attributes;
using MassTransit.AzureServiceBus.Application.IntegrationServices.Contracts.Services.RequestResponse.CQRS;
using MassTransit.AzureServiceBus.Application.RequestResponse.CQRS.CommandNoParam;
using MassTransit.AzureServiceBus.Infrastructure.Eventing;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.MassTransit.RequestResponse.RequestResponse.MapperRequestMessage", Version = "1.0")]

namespace MassTransit.AzureServiceBus.Services.RequestResponse.CQRS
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