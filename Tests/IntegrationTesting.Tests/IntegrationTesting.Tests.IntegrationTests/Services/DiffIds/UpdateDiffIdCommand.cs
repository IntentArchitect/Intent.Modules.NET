using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Services.DiffIds
{
    public class UpdateDiffIdCommand
    {
        public UpdateDiffIdCommand()
        {
            Name = null!;
        }

        public Guid MyId { get; set; }
        public string Name { get; set; }

        public static UpdateDiffIdCommand Create(Guid myId, string name)
        {
            return new UpdateDiffIdCommand
            {
                MyId = myId,
                Name = name
            };
        }
    }
}