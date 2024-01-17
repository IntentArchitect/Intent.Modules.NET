using AdvancedMappingCrud.Repositories.Tests.Application.Common.Mappings;
using AdvancedMappingCrud.Repositories.Tests.Domain;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Products
{
    public class TagDto : IMapFrom<Tag>
    {
        public TagDto()
        {
            Name = null!;
            Value = null!;
        }

        public string Name { get; set; }
        public string Value { get; set; }

        public static TagDto Create(string name, string value)
        {
            return new TagDto
            {
                Name = name,
                Value = value
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Tag, TagDto>();
        }
    }
}