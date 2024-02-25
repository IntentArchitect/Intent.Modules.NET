using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Eventing.MassTransit.RequestResponse.ClientContracts.DtoContract", Version = "2.0")]

namespace MassTransit.AzureServiceBus.Application.IntegrationServices.Contracts.Services.RequestResponse.CQRS
{
    public class CommandDtoReturn
    {
        public CommandDtoReturn()
        {
            Input = null!;
        }

        public string Input { get; set; }

        public static CommandDtoReturn Create(string input)
        {
            return new CommandDtoReturn
            {
                Input = input
            };
        }
    }
}