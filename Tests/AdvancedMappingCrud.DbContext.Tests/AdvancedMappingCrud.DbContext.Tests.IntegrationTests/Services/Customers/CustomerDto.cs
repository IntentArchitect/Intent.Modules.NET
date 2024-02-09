using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.DbContext.Tests.IntegrationTests.Services.Customers
{
    public class CustomerDto
    {
        public CustomerDto()
        {
            Name = null!;
            Surname = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public bool IsActive { get; set; }

        public static CustomerDto Create(Guid id, string name, string surname, bool isActive)
        {
            return new CustomerDto
            {
                Id = id,
                Name = name,
                Surname = surname,
                IsActive = isActive
            };
        }
    }
}