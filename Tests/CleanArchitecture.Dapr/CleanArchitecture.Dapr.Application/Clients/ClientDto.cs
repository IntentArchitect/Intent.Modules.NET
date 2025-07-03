using AutoMapper;
using CleanArchitecture.Dapr.Application.Common.Mappings;
using CleanArchitecture.Dapr.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.Dapr.Application.Clients
{
    public class ClientDto : IMapFrom<Client>
    {
        public ClientDto()
        {
            Name = null!;
            TagsIds = null!;
            Id = null!;
        }

        public string Name { get; set; }
        public string TagsIds { get; set; }
        public string Id { get; set; }

        public static ClientDto Create(string name, string tagsIds, string id)
        {
            return new ClientDto
            {
                Name = name,
                TagsIds = tagsIds,
                Id = id
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Client, ClientDto>();
        }
    }
}