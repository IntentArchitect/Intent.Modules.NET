using AdvancedMappingCrudMongo.Tests.Application.Common.Mappings;
using AdvancedMappingCrudMongo.Tests.Domain.Entities;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrudMongo.Tests.Application.ExternalDocs
{
    public class ExternalDocDto : IMapFrom<ExternalDoc>
    {
        public ExternalDocDto()
        {
            Name = null!;
            Thing = null!;
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Thing { get; set; }

        public static ExternalDocDto Create(long id, string name, string thing)
        {
            return new ExternalDocDto
            {
                Id = id,
                Name = name,
                Thing = thing
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<ExternalDoc, ExternalDocDto>();
        }
    }
}