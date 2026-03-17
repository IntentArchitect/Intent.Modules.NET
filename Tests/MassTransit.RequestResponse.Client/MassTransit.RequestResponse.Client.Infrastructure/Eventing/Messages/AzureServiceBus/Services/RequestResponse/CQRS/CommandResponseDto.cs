using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.MassTransit.RequestResponse.RequestResponse.MapperResponseMessage", Version = "1.0")]

namespace MassTransit.AzureServiceBus.Services.RequestResponse.CQRS
{
    public class CommandResponseDto
    {
        public CommandResponseDto()
        {
        }

        public string Result { get; set; }

        public MassTransit.RequestResponse.Client.Application.IntegrationServices.Contracts.AzureServiceBus.Services.RequestResponse.CQRS.CommandResponseDto ToDto()
        {
            return MassTransit.RequestResponse.Client.Application.IntegrationServices.Contracts.AzureServiceBus.Services.RequestResponse.CQRS.CommandResponseDto.Create(Result);
        }
    }
}