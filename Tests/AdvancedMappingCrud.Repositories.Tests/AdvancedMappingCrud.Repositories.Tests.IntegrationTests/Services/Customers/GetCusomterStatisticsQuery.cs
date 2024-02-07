using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Customers
{
    public class GetCusomterStatisticsQuery
    {
        public Guid CustomerId { get; set; }

        public static GetCusomterStatisticsQuery Create(Guid customerId)
        {
            return new GetCusomterStatisticsQuery
            {
                CustomerId = customerId
            };
        }
    }
}