using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.DtoContract", Version = "2.0")]

namespace ProxyServiceTests.Proxy.AuthHeaderProvider.Application.IntegrationServices.Contracts.OriginalServices.Services.Clients
{
    public class ClientCreateDto
    {

        public string Name { get; set; }

        public static ClientCreateDto Create(string name = "n/a")
        {
            return new ClientCreateDto
            {
                Name = name
            };
        }
    }
}