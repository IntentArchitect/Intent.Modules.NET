using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Services.CheckNewCompChildCruds
{
    public class CreateCheckNewCompChildCrudCommand
    {
        public CreateCheckNewCompChildCrudCommand()
        {
            Name = null!;
        }

        public string Name { get; set; }

        public static CreateCheckNewCompChildCrudCommand Create(string name)
        {
            return new CreateCheckNewCompChildCrudCommand
            {
                Name = name
            };
        }
    }
}