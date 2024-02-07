using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Customers
{
    public class GetCustomersQuery
    {
        public bool IsActive { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }

        public static GetCustomersQuery Create(bool isActive, string? name, string? surname)
        {
            return new GetCustomersQuery
            {
                IsActive = isActive,
                Name = name,
                Surname = surname
            };
        }
    }
}