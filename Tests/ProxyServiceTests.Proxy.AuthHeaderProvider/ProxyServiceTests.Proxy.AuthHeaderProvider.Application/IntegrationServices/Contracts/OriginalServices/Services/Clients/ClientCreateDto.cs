using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.DtoContract", Version = "2.0")]

namespace ProxyServiceTests.Proxy.AuthHeaderProvider.Application.IntegrationServices.Contracts.OriginalServices.Services.Clients
{
    public class ClientCreateDto
    {
        public ClientCreateDto()
        {
            Name = null!;
        }

        public string Name { get; set; }

        public static ClientCreateDto Create(string name)
        {
            return new ClientCreateDto
            {
                Name = name
            };
        }
    }
}