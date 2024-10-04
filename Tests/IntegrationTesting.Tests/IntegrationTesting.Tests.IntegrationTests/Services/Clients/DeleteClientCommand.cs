using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Services.Clients
{
    public class DeleteClientCommand
    {
        public Guid Id { get; set; }

        public static DeleteClientCommand Create(Guid id)
        {
            return new DeleteClientCommand
            {
                Id = id
            };
        }
    }
}