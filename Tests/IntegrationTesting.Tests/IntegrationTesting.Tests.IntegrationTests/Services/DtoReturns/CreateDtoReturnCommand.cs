using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Services.DtoReturns
{
    public class CreateDtoReturnCommand
    {
        public CreateDtoReturnCommand()
        {
            Name = null!;
        }

        public string Name { get; set; }

        public static CreateDtoReturnCommand Create(string name)
        {
            return new CreateDtoReturnCommand
            {
                Name = name
            };
        }
    }
}