using AutoMapper;
using CosmosDB.PrivateSetters.Application.Common.Mappings;
using CosmosDB.PrivateSetters.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CosmosDB.PrivateSetters.Application.Regions
{
    public class RegionDto : IMapFrom<Region>
    {
        public RegionDto()
        {
            Id = null!;
            Name = null!;
        }

        public string Id { get; set; }
        public string Name { get; set; }

        public static RegionDto Create(string id, string name)
        {
            return new RegionDto
            {
                Id = id,
                Name = name
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Region, RegionDto>();
        }
    }
}