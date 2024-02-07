using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Services.Parents
{
    public class DeleteParentCommand
    {
        public Guid Id { get; set; }

        public static DeleteParentCommand Create(Guid id)
        {
            return new DeleteParentCommand
            {
                Id = id
            };
        }
    }
}