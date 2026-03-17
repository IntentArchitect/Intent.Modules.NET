using Intent.RoslynWeaver.Attributes;
using MassTransit.RequestResponse.Client.Application.IntegrationServices.Contracts.AzureServiceBus.Services.RequestResponse.CQRS;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.MassTransit.RequestResponse.RequestResponse.MapperRequestMessage", Version = "1.0")]

namespace MassTransit.AzureServiceBus.Services.RequestResponse.CQRS
{
    public class CommandNoParam
    {
        public CommandNoParam()
        {
        }

        public CommandNoParam(MassTransit.RequestResponse.Client.Application.IntegrationServices.Contracts.AzureServiceBus.Services.RequestResponse.CQRS.CommandNoParam dto)
        {
        }
    }
}