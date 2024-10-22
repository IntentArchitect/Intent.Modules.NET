using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace Ardalis.IntegrationTests.Services.Clients
{
    public class GetClientsQuery
    {
        public static GetClientsQuery Create()
        {
            return new GetClientsQuery
            {
            };
        }
    }
}