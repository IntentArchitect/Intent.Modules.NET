using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.Services.Parents
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