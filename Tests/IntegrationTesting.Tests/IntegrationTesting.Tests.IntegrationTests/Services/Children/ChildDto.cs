using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Services.Children
{
    public class ChildDto
    {
        public ChildDto()
        {
            Name = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid MyParentId { get; set; }

        public static ChildDto Create(Guid id, string name, Guid myParentId)
        {
            return new ChildDto
            {
                Id = id,
                Name = name,
                MyParentId = myParentId
            };
        }
    }
}