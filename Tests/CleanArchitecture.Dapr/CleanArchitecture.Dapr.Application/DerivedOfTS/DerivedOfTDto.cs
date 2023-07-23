using AutoMapper;
using CleanArchitecture.Dapr.Application.Common.Mappings;
using CleanArchitecture.Dapr.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.Dapr.Application.DerivedOfTS
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
        public int GenericTypeAttribute { get; set; }

        public static DerivedOfTDto Create(string id, string derivedAttribute, int genericTypeAttribute)
        {
            return new DerivedOfTDto
            {
                Id = id,
                DerivedAttribute = derivedAttribute,
                GenericTypeAttribute = genericTypeAttribute
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<DerivedOfT, DerivedOfTDto>()
                .ForMember(d => d.GenericTypeAttribute, opt => opt.MapFrom(src => (int)src.GenericTypeAttribute));
        }
    }
}