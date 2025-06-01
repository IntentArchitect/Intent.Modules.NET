using AdvancedMappingCrud.Cosmos.Tests.Application.Common.Mappings;
using AdvancedMappingCrud.Cosmos.Tests.Domain;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.EmbeddedParents
{
    public class EmbeddedEmbeddedParentEmbeddedChildDto : IMapFrom<EmbeddedChild>
    {
        public EmbeddedEmbeddedParentEmbeddedChildDto()
        {
            Name = null!;
        }

        public string Name { get; set; }
        public int Age { get; set; }

        public static EmbeddedEmbeddedParentEmbeddedChildDto Create(string name, int age)
        {
            return new EmbeddedEmbeddedParentEmbeddedChildDto
            {
                Name = name,
                Age = age
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<EmbeddedChild, EmbeddedEmbeddedParentEmbeddedChildDto>();
        }
    }
}