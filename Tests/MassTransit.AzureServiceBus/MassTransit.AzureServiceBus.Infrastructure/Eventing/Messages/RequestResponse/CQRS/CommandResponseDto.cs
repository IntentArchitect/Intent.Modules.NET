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

        public CommandResponseDto(MassTransit.AzureServiceBus.Application.RequestResponse.CQRS.CommandResponseDto dto)
        {
            Result = dto.Result;
        }

        public string Result { get; set; }

        public MassTransit.AzureServiceBus.Application.IntegrationServices.Contracts.Services.RequestResponse.CQRS.CommandResponseDto ToDto()
        {
            return MassTransit.AzureServiceBus.Application.IntegrationServices.Contracts.Services.RequestResponse.CQRS.CommandResponseDto.Create(Result);
        }
    }
}