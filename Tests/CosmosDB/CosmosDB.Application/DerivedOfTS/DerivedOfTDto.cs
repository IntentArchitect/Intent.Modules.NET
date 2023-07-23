using AutoMapper;
using CosmosDB.Application.Common.Mappings;
using CosmosDB.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CosmosDB.Application.DerivedOfTS
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
        public int GenericAttribute { get; set; }

        public static DerivedOfTDto Create(string id, string derivedAttribute, int genericAttribute)
        {
            return new DerivedOfTDto
            {
                Id = id,
                DerivedAttribute = derivedAttribute,
                GenericAttribute = genericAttribute
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<DerivedOfT, DerivedOfTDto>()
                .ForMember(d => d.GenericAttribute, opt => opt.MapFrom(src => (int)src.GenericAttribute));
        }
    }
}