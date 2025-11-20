using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CompositePublishTest.Application.Clients
{
    public class ClientDto
    {
        public ClientDto()
        {
            Name = null!;
            Location = null!;
            Description = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }

        public static ClientDto Create(Guid id, string name, string location, string description)
        {
            return new ClientDto
            {
                Id = id,
                Name = name,
                Location = location,
                Description = description
            };
        }
    }
}