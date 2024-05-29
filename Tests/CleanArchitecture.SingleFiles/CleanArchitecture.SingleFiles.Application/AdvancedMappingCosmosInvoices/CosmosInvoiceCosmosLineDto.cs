using AutoMapper;
using CleanArchitecture.SingleFiles.Application.Common.Mappings;
using CleanArchitecture.SingleFiles.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.SingleFiles.Application.AdvancedMappingCosmosInvoices
{
    public class CosmosInvoiceCosmosLineDto : IMapFrom<CosmosLine>
    {
        public CosmosInvoiceCosmosLineDto()
        {
            CosmosInvoiceId = null!;
            Id = null!;
            Name = null!;
        }

        public string CosmosInvoiceId { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }

        public static CosmosInvoiceCosmosLineDto Create(string cosmosInvoiceId, string id, string name)
        {
            return new CosmosInvoiceCosmosLineDto
            {
                CosmosInvoiceId = cosmosInvoiceId,
                Id = id,
                Name = name
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<CosmosLine, CosmosInvoiceCosmosLineDto>();
        }
    }
}