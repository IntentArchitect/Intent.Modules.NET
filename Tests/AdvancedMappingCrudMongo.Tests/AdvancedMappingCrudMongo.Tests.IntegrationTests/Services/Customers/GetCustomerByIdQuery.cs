using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrudMongo.Tests.IntegrationTests.Services.Customers
{
    public class GetCustomerByIdQuery
    {
        public GetCustomerByIdQuery()
        {
            Id = null!;
        }

        public string Id { get; set; }

        public static GetCustomerByIdQuery Create(string id)
        {
            return new GetCustomerByIdQuery
            {
                Id = id
            };
        }
    }
}