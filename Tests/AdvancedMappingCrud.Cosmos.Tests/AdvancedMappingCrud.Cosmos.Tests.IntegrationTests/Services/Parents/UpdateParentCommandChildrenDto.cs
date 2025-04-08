using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.Services.Parents
{
    public class UpdateParentCommandChildrenDto
    {
        public UpdateParentCommandChildrenDto()
        {
            Id = null!;
            Name = null!;
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }

        public static UpdateParentCommandChildrenDto Create(string id, string name, int age)
        {
            return new UpdateParentCommandChildrenDto
            {
                Id = id,
                Name = name,
                Age = age
            };
        }
    }
}