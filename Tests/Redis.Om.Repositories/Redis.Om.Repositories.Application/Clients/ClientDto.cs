using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using Redis.Om.Repositories.Application.Common.Mappings;
using Redis.Om.Repositories.Domain;
using Redis.Om.Repositories.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Redis.Om.Repositories.Application.Clients
{
    public class ClientDto : IMapFrom<Client>
    {
        public ClientDto()
        {
            Id = null!;
            Name = null!;
        }

        public string Id { get; set; }
        public ClientType Type { get; set; }
        public string Name { get; set; }

        public static ClientDto Create(string id, ClientType type, string name)
        {
            return new ClientDto
            {
                Id = id,
                Type = type,
                Name = name
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Client, ClientDto>();
        }
    }
}