using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Services.Brands
{
    public class CreateBrandCommand
    {
        public CreateBrandCommand()
        {
            Name = null!;
        }

        public string Name { get; set; }

        public static CreateBrandCommand Create(string name)
        {
            return new CreateBrandCommand
            {
                Name = name
            };
        }
    }
}