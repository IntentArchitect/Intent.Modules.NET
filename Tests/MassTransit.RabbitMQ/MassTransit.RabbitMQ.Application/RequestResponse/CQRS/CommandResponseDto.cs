using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace MassTransit.RabbitMQ.Application.RequestResponse.CQRS
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