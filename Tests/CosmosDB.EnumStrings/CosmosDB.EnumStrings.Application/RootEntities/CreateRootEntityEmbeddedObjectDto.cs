using CosmosDB.EnumStrings.Domain;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CosmosDB.EnumStrings.Application.RootEntities
{
    public class CreateRootEntityEmbeddedObjectDto
    {
        public CreateRootEntityEmbeddedObjectDto()
        {
            Name = null!;
        }

        public string Name { get; set; }
        public EnumExample EnumExample { get; set; }
        public EnumExample NullableEnumExample { get; set; }

        public static CreateRootEntityEmbeddedObjectDto Create(
            string name,
            EnumExample enumExample,
            EnumExample nullableEnumExample)
        {
            return new CreateRootEntityEmbeddedObjectDto
            {
                Name = name,
                EnumExample = enumExample,
                NullableEnumExample = nullableEnumExample
            };
        }
    }
}