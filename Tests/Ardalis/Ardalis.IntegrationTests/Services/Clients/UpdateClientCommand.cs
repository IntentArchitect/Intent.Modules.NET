using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace Ardalis.IntegrationTests.Services.Clients
{
    public class UpdateClientCommand
    {
        public UpdateClientCommand()
        {
            Name = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }

        public static UpdateClientCommand Create(Guid id, string name)
        {
            return new UpdateClientCommand
            {
                Id = id,
                Name = name
            };
        }
    }
}