using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Services.NoReturns
{
    public class UpdateNoReturnCommand
    {
        public UpdateNoReturnCommand()
        {
            Name = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }

        public static UpdateNoReturnCommand Create(Guid id, string name)
        {
            return new UpdateNoReturnCommand
            {
                Id = id,
                Name = name
            };
        }
    }
}