using AutoMapper;
using CosmosDB.PrivateSetters.Application.Common.Mappings;
using CosmosDB.PrivateSetters.Domain;
using CosmosDB.PrivateSetters.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CosmosDB.PrivateSetters.Application.Clients
{
    public class ClientDto : IMapFrom<Client>
    {
        public ClientDto()
        {
            Identifier = null!;
            Name = null!;
        }

        public string Identifier { get; set; }
        public ClientType Type { get; set; }
        public string Name { get; set; }

        public static ClientDto Create(string identifier, ClientType type, string name)
        {
            return new ClientDto
            {
                Identifier = identifier,
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