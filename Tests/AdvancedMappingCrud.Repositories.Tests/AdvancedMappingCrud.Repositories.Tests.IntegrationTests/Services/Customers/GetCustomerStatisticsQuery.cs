using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Customers
{
    public class GetCustomerStatisticsQuery
    {
        public Guid CustomerId { get; set; }

        public static GetCustomerStatisticsQuery Create(Guid customerId)
        {
            return new GetCustomerStatisticsQuery
            {
                CustomerId = customerId
            };
        }
    }
}