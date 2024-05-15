using System.Collections.Generic;
using AutoMapper;
using CleanArchitecture.Dapr.Application.Common.Mappings;
using CleanArchitecture.Dapr.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.Dapr.Application.AdvancedMappingSystem.Clients
{
    public class ClientDto : IMapFrom<Client>
    {
        public ClientDto()
        {
            Id = null!;
            Name = null!;
            TagsIds = null!;
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public List<string> TagsIds { get; set; }

        public static ClientDto Create(string id, string name, List<string> tagsIds)
        {
            return new ClientDto
            {
                Id = id,
                Name = name,
                TagsIds = tagsIds
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Client, ClientDto>();
        }
    }
}