using System;
using Ardalis.Application.Common.Mappings;
using Ardalis.Domain.Entities;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Ardalis.Application.Clients
{
    public class ClientDto : IMapFrom<Client>
    {
        public ClientDto()
        {
            Name = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }

        public static ClientDto Create(Guid id, string name)
        {
            return new ClientDto
            {
                Id = id,
                Name = name
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Client, ClientDto>();
        }
    }
}