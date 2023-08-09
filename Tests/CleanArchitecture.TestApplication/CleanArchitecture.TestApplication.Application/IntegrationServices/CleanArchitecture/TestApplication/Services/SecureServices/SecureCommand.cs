using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.DtoContract", Version = "2.0")]

namespace CleanArchitecture.TestApplication.Application.IntegrationServices.CleanArchitecture.TestApplication.Services.SecureServices
{
    public class SecureCommand
    {
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