using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Customers
{
    public class GetCustomerByIdQuery
    {
        public Guid Id { get; set; }

        public static GetCustomerByIdQuery Create(Guid id)
        {
            return new GetCustomerByIdQuery
            {
                Id = id
            };
        }
    }
}