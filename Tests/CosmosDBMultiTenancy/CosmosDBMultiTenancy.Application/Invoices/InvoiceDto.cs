using AutoMapper;
using CosmosDBMultiTenancy.Application.Common.Mappings;
using CosmosDBMultiTenancy.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CosmosDBMultiTenancy.Application.Invoices
{
    public class InvoiceDto : IMapFrom<Invoice>
    {
        public InvoiceDto()
        {
            Id = null!;
            Number = null!;
        }

        public string Id { get; set; }
        public string Number { get; set; }

        public static InvoiceDto Create(string id, string number)
        {
            return new InvoiceDto
            {
                Id = id,
                Number = number
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Invoice, InvoiceDto>();
        }
    }
}