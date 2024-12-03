using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.Common.Mappings;
using MongoDb.TestApplication.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace MongoDb.TestApplication.Application.DerivedOfTS
{
    public class DerivedOfTDto : IMapFrom<DerivedOfT>
    {
        public DerivedOfTDto()
        {
            Id = null!;
            DerivedAttribute = null!;
        }

        public string Id { get; set; }
        public string DerivedAttribute { get; set; }
        public int BaseAttribute { get; set; }

        public static DerivedOfTDto Create(string id, string derivedAttribute, int baseAttribute)
        {
            return new DerivedOfTDto
            {
                Id = id,
                DerivedAttribute = derivedAttribute,
                BaseAttribute = baseAttribute
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<DerivedOfT, DerivedOfTDto>()
                .ForMember(d => d.BaseAttribute, opt => opt.MapFrom(src => (int)src.BaseAttribute));
        }
    }
}