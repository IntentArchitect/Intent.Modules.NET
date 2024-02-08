using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Services.Customers
{
    public class UpdateCustomerCommand
    {
        public UpdateCustomerCommand()
        {
            Name = null!;
            Line1 = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Line1 { get; set; }

        public static UpdateCustomerCommand Create(Guid id, string name, string line1)
        {
            return new UpdateCustomerCommand
            {
                Id = id,
                Name = name,
                Line1 = line1
            };
        }
    }
}