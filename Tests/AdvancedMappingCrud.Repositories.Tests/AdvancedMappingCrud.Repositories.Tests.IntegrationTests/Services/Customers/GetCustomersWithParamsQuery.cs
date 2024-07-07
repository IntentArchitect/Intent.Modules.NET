using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Customers
{
    public class GetCustomersWithParamsQuery
    {
        public bool IsActive { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }

        public static GetCustomersWithParamsQuery Create(bool isActive, string? name, string? surname)
        {
            return new GetCustomersWithParamsQuery
            {
                IsActive = isActive,
                Name = name,
                Surname = surname
            };
        }
    }
}