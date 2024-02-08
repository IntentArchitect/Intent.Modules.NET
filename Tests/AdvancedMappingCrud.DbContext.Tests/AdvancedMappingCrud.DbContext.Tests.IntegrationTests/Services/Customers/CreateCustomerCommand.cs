using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.DbContext.Tests.IntegrationTests.Services.Customers
{
    public class CreateCustomerCommand
    {
        public CreateCustomerCommand()
        {
            Name = null!;
            Surname = null!;
        }

        public string Name { get; set; }
        public string Surname { get; set; }
        public bool IsActive { get; set; }

        public static CreateCustomerCommand Create(string name, string surname, bool isActive)
        {
            return new CreateCustomerCommand
            {
                Name = name,
                Surname = surname,
                IsActive = isActive
            };
        }
    }
}