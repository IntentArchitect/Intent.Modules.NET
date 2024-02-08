using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Services.Parents
{
    public class ParentDto
    {
        public ParentDto()
        {
            Name = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }

        public static ParentDto Create(Guid id, string name)
        {
            return new ParentDto
            {
                Id = id,
                Name = name
            };
        }
    }
}