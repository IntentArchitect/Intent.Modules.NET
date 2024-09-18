using AutoMapper;
using CosmosDB.EnumStrings.Application.Common.Mappings;
using CosmosDB.EnumStrings.Domain;
using CosmosDB.EnumStrings.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CosmosDB.EnumStrings.Application.RootEntities
{
    public class RootEntityNestedEntityDto : IMapFrom<NestedEntity>
    {
        public RootEntityNestedEntityDto()
        {
            Name = null!;
            Id = null!;
            EmbeddedObject = null!;
        }

        public string Name { get; set; }
        public EnumExample EnumExample { get; set; }
        public EnumExample? NullableEnumExample { get; set; }
        public string Id { get; set; }
        public RootEntityNestedEntityEmbeddedObjectDto EmbeddedObject { get; set; }

        public static RootEntityNestedEntityDto Create(
            string name,
            EnumExample enumExample,
            EnumExample? nullableEnumExample,
            string id,
            RootEntityNestedEntityEmbeddedObjectDto embeddedObject)
        {
            return new RootEntityNestedEntityDto
            {
                Name = name,
                EnumExample = enumExample,
                NullableEnumExample = nullableEnumExample,
                Id = id,
                EmbeddedObject = embeddedObject
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<NestedEntity, RootEntityNestedEntityDto>();
        }
    }
}