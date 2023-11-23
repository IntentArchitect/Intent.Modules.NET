using System;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using Standard.AspNetCore.TestApplication.Application.Common.Mappings;
using Standard.AspNetCore.TestApplication.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Standard.AspNetCore.TestApplication.Application.Clients
{
    public class Client : IMapFrom<Domain.Entities.Client>
    {
        public Client()
        {
        }

        public Guid Id { get; set; }

        public static Client Create(Guid id)
        {
            return new Client
            {
                Id = id
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Domain.Entities.Client, Client>();
        }
    }
}