using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.Parents
{
    public class CreateParentCommandChildrenDto
    {
        public CreateParentCommandChildrenDto()
        {
            Name = null!;
        }

        public string Name { get; set; }
        public int Age { get; set; }

        public static CreateParentCommandChildrenDto Create(string name, int age)
        {
            return new CreateParentCommandChildrenDto
            {
                Name = name,
                Age = age
            };
        }
    }
}