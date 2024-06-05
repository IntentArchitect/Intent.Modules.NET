using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace ProxyServiceTests.OriginalServices.Application.Clients
{
    public class ClientUpdateDto
    {
        public ClientUpdateDto()
        {
            Name = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }

        public static ClientUpdateDto Create(Guid id, string name)
        {
            return new ClientUpdateDto
            {
                Id = id,
                Name = name
            };
        }
    }
}