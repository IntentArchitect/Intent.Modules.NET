using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Services.NoReturns
{
    public class CreateNoReturnCommand
    {
        public CreateNoReturnCommand()
        {
            Name = null!;
        }

        public string Name { get; set; }

        public static CreateNoReturnCommand Create(string name)
        {
            return new CreateNoReturnCommand
            {
                Name = name
            };
        }
    }
}