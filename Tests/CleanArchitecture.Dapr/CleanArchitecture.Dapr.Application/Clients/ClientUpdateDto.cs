using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.Dapr.Application.Clients
{
    public class ClientUpdateDto
    {
        public ClientUpdateDto()
        {
            Id = null!;
            Name = null!;
            TagsIds = null!;
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public List<string> TagsIds { get; set; }

        public static ClientUpdateDto Create(string id, string name, List<string> tagsIds)
        {
            return new ClientUpdateDto
            {
                Id = id,
                Name = name,
                TagsIds = tagsIds
            };
        }
    }
}