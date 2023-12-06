using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace SignalR.Application
{
    public class MessageToClientDto
    {
        public MessageToClientDto()
        {
            Message = null!;
        }

        public string Message { get; set; }

        public static MessageToClientDto Create(string message)
        {
            return new MessageToClientDto
            {
                Message = message
            };
        }
    }
}