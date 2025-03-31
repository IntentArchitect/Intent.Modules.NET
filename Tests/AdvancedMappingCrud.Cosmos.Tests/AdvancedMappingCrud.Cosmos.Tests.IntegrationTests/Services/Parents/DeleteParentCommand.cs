using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.Services.Parents
{
    public class DeleteParentCommand
    {
        public DeleteParentCommand()
        {
            Id = null!;
        }

        public string Id { get; set; }

        public static DeleteParentCommand Create(string id)
        {
            return new DeleteParentCommand
            {
                Id = id
            };
        }
    }
}