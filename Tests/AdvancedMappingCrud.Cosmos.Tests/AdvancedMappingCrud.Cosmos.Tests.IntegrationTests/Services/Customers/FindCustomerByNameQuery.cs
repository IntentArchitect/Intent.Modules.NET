using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.Services.Customers
{
    public class FindCustomerByNameQuery
    {
        public FindCustomerByNameQuery()
        {
            Name = null!;
        }

        public string Name { get; set; }

        public static FindCustomerByNameQuery Create(string name)
        {
            return new FindCustomerByNameQuery
            {
                Name = name
            };
        }
    }
}