using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.DBAssigneds
{
    public class UpdateDBAssignedCommand
    {
        public UpdateDBAssignedCommand()
        {
            Name = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }

        public static UpdateDBAssignedCommand Create(Guid id, string name)
        {
            return new UpdateDBAssignedCommand
            {
                Id = id,
                Name = name
            };
        }
    }
}