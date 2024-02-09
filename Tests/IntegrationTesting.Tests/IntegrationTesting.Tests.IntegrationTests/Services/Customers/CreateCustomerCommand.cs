using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Services.Customers
{
    public class CreateCustomerCommand
    {
        public CreateCustomerCommand()
        {
            Name = null!;
            Line1 = null!;
        }

        public string Name { get; set; }
        public string Line1 { get; set; }

        public static CreateCustomerCommand Create(string name, string line1)
        {
            return new CreateCustomerCommand
            {
                Name = name,
                Line1 = line1
            };
        }
    }
}