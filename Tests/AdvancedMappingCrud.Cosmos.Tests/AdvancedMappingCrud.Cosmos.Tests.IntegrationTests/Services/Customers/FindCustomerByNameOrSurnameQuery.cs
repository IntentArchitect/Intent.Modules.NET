using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.Services.Customers
{
    public class FindCustomerByNameOrSurnameQuery
    {
        public string? Name { get; set; }
        public string? Surname { get; set; }

        public static FindCustomerByNameOrSurnameQuery Create(string? name, string? surname)
        {
            return new FindCustomerByNameOrSurnameQuery
            {
                Name = name,
                Surname = surname
            };
        }
    }
}