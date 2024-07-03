using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.DtoContract", Version = "2.0")]

namespace ProxyServiceTests.Proxy.TMS.Application.IntegrationServices.Contracts.OriginalServices.Services.Clients
{
    public class ClientDto
    {
        public ClientDto()
        {
            Name = null!;
        }

        public string Name { get; set; }
        public Guid Id { get; set; }

        public static ClientDto Create(string name, Guid id)
        {
            return new ClientDto
            {
                Name = name,
                Id = id
            };
        }
    }
}