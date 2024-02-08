using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Customers
{
    public class GetCustomersByNameAndSurnameQuery
    {
        public GetCustomersByNameAndSurnameQuery()
        {
            Name = null!;
            Surname = null!;
        }

        public string Name { get; set; }
        public string Surname { get; set; }

        public static GetCustomersByNameAndSurnameQuery Create(string name, string surname)
        {
            return new GetCustomersByNameAndSurnameQuery
            {
                Name = name,
                Surname = surname
            };
        }
    }
}