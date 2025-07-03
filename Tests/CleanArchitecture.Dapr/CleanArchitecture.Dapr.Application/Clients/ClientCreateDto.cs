using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.Dapr.Application.Clients
{
    public class ClientCreateDto
    {
        public ClientCreateDto()
        {
            Name = null!;
            TagsIds = null!;
        }

        public string Name { get; set; }
        public List<string> TagsIds { get; set; }

        public static ClientCreateDto Create(string name, List<string> tagsIds)
        {
            return new ClientCreateDto
            {
                Name = name,
                TagsIds = tagsIds
            };
        }
    }
}