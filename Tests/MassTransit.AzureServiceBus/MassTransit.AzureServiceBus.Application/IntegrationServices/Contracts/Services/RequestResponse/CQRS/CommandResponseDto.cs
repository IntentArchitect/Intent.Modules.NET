using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Eventing.MassTransit.RequestResponse.ClientContracts.DtoContract", Version = "2.0")]

namespace MassTransit.AzureServiceBus.Application.IntegrationServices.Contracts.Services.RequestResponse.CQRS
{
    public class CommandResponseDto
    {
        public CommandResponseDto()
        {
            Result = null!;
        }

        public string Result { get; set; }

        public static CommandResponseDto Create(string result)
        {
            return new CommandResponseDto
            {
                Result = result
            };
        }
    }
}