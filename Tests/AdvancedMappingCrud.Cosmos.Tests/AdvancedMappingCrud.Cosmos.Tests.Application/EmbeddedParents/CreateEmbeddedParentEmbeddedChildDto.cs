using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.EmbeddedParents
{
    public class CreateEmbeddedParentEmbeddedChildDto
    {
        public CreateEmbeddedParentEmbeddedChildDto()
        {
            Name = null!;
        }

        public string Name { get; set; }
        public int Age { get; set; }

        public static CreateEmbeddedParentEmbeddedChildDto Create(string name, int age)
        {
            return new CreateEmbeddedParentEmbeddedChildDto
            {
                Name = name,
                Age = age
            };
        }
    }
}