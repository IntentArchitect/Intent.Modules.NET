using System;
using System.Collections.Generic;
using AutoMapper;
using CosmosDB.Application.Common.Mappings;
using CosmosDB.Domain;
using CosmosDB.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CosmosDB.Application.Invoices
{
    public class InvoiceClientDto : IMapFrom<Client>
    {
        public InvoiceClientDto()
        {
            Id = null!;
        }

        public ClientType Type { get; set; }
        public string Id { get; set; }

        public static InvoiceClientDto Create(ClientType type, string id)
        {
            return new InvoiceClientDto
            {
                Type = type,
                Id = id
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Client, InvoiceClientDto>()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Identifier));
        }
    }
}