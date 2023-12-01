using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace SignalR.Application
{
    public class MassageToClientDto
    {
        public MassageToClientDto()
        {
            Message = null!;
        }

        public string Message { get; set; }

        public static MassageToClientDto Create(string message)
        {
            return new MassageToClientDto
            {
                Message = message
            };
        }
    }
}