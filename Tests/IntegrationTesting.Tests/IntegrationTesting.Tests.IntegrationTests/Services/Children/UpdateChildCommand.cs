using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Services.Children
{
    public class UpdateChildCommand
    {
        public UpdateChildCommand()
        {
            Name = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid MyParentId { get; set; }

        public static UpdateChildCommand Create(Guid id, string name, Guid myParentId)
        {
            return new UpdateChildCommand
            {
                Id = id,
                Name = name,
                MyParentId = myParentId
            };
        }
    }
}