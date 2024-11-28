using System;
using AspNetCoreMvc.Application.Common.Mappings;
using AspNetCoreMvc.Domain.Entities;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AspNetCoreMvc.Application.ClientsService
{
    public class ClientDto : IMapFrom<Client>
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

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Client, ClientDto>();
        }
    }
}