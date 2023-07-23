using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.Common.Mappings;
using MongoDb.TestApplication.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace MongoDb.TestApplication.Application.Deriveds
{
    public class DerivedDto : IMapFrom<Derived>
    {
        public DerivedDto()
        {
            Id = null!;
            DerivedAttribute = null!;
            BaseAttribute = null!;
        }

        public string Id { get; set; }
        public string DerivedAttribute { get; set; }
        public string BaseAttribute { get; set; }

        public static DerivedDto Create(string id, string derivedAttribute, string baseAttribute)
        {
            return new DerivedDto
            {
                Id = id,
                DerivedAttribute = derivedAttribute,
                BaseAttribute = baseAttribute
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Derived, DerivedDto>();
        }
    }
}