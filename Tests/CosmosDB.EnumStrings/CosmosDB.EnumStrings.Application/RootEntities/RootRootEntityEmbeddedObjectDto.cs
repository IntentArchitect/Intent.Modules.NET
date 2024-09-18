using AutoMapper;
using CosmosDB.EnumStrings.Application.Common.Mappings;
using CosmosDB.EnumStrings.Domain;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CosmosDB.EnumStrings.Application.RootEntities
{
    public class RootRootEntityEmbeddedObjectDto : IMapFrom<EmbeddedObject>
    {
        public RootRootEntityEmbeddedObjectDto()
        {
            Name = null!;
        }

        public string Name { get; set; }
        public EnumExample EnumExample { get; set; }
        public EnumExample NullableEnumExample { get; set; }

        public static RootRootEntityEmbeddedObjectDto Create(
            string name,
            EnumExample enumExample,
            EnumExample nullableEnumExample)
        {
            return new RootRootEntityEmbeddedObjectDto
            {
                Name = name,
                EnumExample = enumExample,
                NullableEnumExample = nullableEnumExample
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<EmbeddedObject, RootRootEntityEmbeddedObjectDto>();
        }
    }
}