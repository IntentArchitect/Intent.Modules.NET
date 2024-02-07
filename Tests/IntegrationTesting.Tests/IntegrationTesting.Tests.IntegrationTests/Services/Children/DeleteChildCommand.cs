using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Services.Children
{
    public class DeleteChildCommand
    {
        public Guid Id { get; set; }

        public static DeleteChildCommand Create(Guid id)
        {
            return new DeleteChildCommand
            {
                Id = id
            };
        }
    }
}