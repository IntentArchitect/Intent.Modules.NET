using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Services.CheckNewCompChildCruds
{
    public class UpdateCheckNewCompChildCrudCommand
    {
        public UpdateCheckNewCompChildCrudCommand()
        {
            Name = null!;
        }

        public string Name { get; set; }
        public Guid Id { get; set; }

        public static UpdateCheckNewCompChildCrudCommand Create(string name, Guid id)
        {
            return new UpdateCheckNewCompChildCrudCommand
            {
                Name = name,
                Id = id
            };
        }
    }
}