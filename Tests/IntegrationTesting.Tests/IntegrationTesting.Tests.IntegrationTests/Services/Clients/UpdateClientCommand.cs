using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Services.Clients
{
    public class UpdateClientCommand
    {
        public UpdateClientCommand()
        {
            Name = null!;
        }

        public string Name { get; set; }
        public Guid Id { get; set; }

        public static UpdateClientCommand Create(string name, Guid id)
        {
            return new UpdateClientCommand
            {
                Name = name,
                Id = id
            };
        }
    }
}