using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.DtoContract", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.IntegrationServices.SecureServicesService
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