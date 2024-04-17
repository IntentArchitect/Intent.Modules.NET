using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.Common.Mappings;
using MongoDb.TestApplication.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace MongoDb.TestApplication.Application.MapperRoots
{
    public class MapperM2MDto : IMapFrom<MapperM2M>
    {
        public MapperM2MDto()
        {
            Desc = null!;
            Id = null!;
        }

        public string Desc { get; set; }
        public string Id { get; set; }

        public static MapperM2MDto Create(string desc, string id)
        {
            return new MapperM2MDto
            {
                Desc = desc,
                Id = id
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<MapperM2M, MapperM2MDto>();
        }
    }
}