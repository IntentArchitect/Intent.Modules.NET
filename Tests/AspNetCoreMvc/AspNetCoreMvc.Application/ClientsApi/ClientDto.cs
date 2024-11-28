using System;
using AspNetCoreMvc.Application.Common.Mappings;
using AspNetCoreMvc.Domain.Entities;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AspNetCoreMvc.Application.ClientsApi
{
    public class ClientDto : IMapFrom<Client>
    {
        public ClientDto()
        {
        }

        public Guid Id { get; set; }
        public string? Name { get; set; }

        public static ClientDto Create(Guid id, string? name)
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