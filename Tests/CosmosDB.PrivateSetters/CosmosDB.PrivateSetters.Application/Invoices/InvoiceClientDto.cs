using AutoMapper;
using CosmosDB.PrivateSetters.Application.Common.Mappings;
using CosmosDB.PrivateSetters.Domain;
using CosmosDB.PrivateSetters.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CosmosDB.PrivateSetters.Application.Invoices
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