using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.DtoContract", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.IntegrationServices.Services.SecureServices
{
    public class SecureCommand
    {
        public SecureCommand()
        {
            Message = null!;
        }
        public string Message { get; set; }

        public static SecureCommand Create(string message)
        {
            return new SecureCommand
            {
                Message = message
            };
        }
    }
}