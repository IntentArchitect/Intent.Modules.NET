using AutoMapper;
using CleanArchitecture.Dapr.Application.Common.Mappings;
using CleanArchitecture.Dapr.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.Dapr.Application.OldMappingSystem.Deriveds
{
    public class DerivedDto : IMapFrom<Derived>
    {
        public DerivedDto()
        {
            Id = null!;
            Attribute = null!;
            BaseAttribute = null!;
        }

        public string Id { get; set; }
        public string Attribute { get; set; }
        public string BaseAttribute { get; set; }

        public static DerivedDto Create(string id, string attribute, string baseAttribute)
        {
            return new DerivedDto
            {
                Id = id,
                Attribute = attribute,
                BaseAttribute = baseAttribute
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Derived, DerivedDto>();
        }
    }
}