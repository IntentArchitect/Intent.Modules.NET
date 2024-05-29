using AutoMapper;
using CleanArchitecture.SingleFiles.Application.Common.Mappings;
using CleanArchitecture.SingleFiles.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.SingleFiles.Application.AdvancedMappingCosmosInvoices
{
    public class CosmosInvoiceDto : IMapFrom<CosmosInvoice>
    {
        public CosmosInvoiceDto()
        {
            Id = null!;
            Description = null!;
        }

        public string Id { get; set; }
        public string Description { get; set; }

        public static CosmosInvoiceDto Create(string id, string description)
        {
            return new CosmosInvoiceDto
            {
                Id = id,
                Description = description
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<CosmosInvoice, CosmosInvoiceDto>();
        }
    }
}