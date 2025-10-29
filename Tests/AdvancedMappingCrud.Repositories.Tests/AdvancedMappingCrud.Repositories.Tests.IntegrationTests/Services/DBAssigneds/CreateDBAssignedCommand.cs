using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.DBAssigneds
{
    public class CreateDBAssignedCommand
    {
        public CreateDBAssignedCommand()
        {
            Name = null!;
        }

        public string Name { get; set; }

        public static CreateDBAssignedCommand Create(string name)
        {
            return new CreateDBAssignedCommand
            {
                Name = name
            };
        }
    }
}