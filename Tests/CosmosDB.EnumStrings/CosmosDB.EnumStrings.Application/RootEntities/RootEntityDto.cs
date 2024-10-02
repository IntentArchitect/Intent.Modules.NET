using System.Collections.Generic;
using AutoMapper;
using CosmosDB.EnumStrings.Application.Common.Mappings;
using CosmosDB.EnumStrings.Domain;
using CosmosDB.EnumStrings.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CosmosDB.EnumStrings.Application.RootEntities
{
    public class RootEntityDto : IMapFrom<RootEntity>
    {
        public RootEntityDto()
        {
            Id = null!;
            Name = null!;
            Embedded = null!;
            NestedEntities = null!;
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public EnumExample EnumExample { get; set; }
        public EnumExample? NullableEnumExample { get; set; }
        public RootRootEntityEmbeddedObjectDto Embedded { get; set; }
        public List<RootEntityNestedEntityDto> NestedEntities { get; set; }

        public static RootEntityDto Create(
            string id,
            string name,
            EnumExample enumExample,
            EnumExample? nullableEnumExample,
            RootRootEntityEmbeddedObjectDto embedded,
            List<RootEntityNestedEntityDto> nestedEntities)
        {
            return new RootEntityDto
            {
                Id = id,
                Name = name,
                EnumExample = enumExample,
                NullableEnumExample = nullableEnumExample,
                Embedded = embedded,
                NestedEntities = nestedEntities
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<RootEntity, RootEntityDto>()
                .ForMember(d => d.NestedEntities, opt => opt.MapFrom(src => src.NestedEntities));
        }
    }
}