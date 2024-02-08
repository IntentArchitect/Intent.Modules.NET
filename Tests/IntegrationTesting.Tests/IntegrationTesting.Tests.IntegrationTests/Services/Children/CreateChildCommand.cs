using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Services.Children
{
    public class CreateChildCommand
    {
        public CreateChildCommand()
        {
            Name = null!;
        }

        public string Name { get; set; }
        public Guid MyParentId { get; set; }

        public static CreateChildCommand Create(string name, Guid myParentId)
        {
            return new CreateChildCommand
            {
                Name = name,
                MyParentId = myParentId
            };
        }
    }
}