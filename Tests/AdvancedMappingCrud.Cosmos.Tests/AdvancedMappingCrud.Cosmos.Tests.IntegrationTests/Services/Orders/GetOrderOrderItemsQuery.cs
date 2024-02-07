using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.Services.Orders
{
    public class GetOrderOrderItemsQuery
    {
        public GetOrderOrderItemsQuery()
        {
            OrderId = null!;
        }

        public string OrderId { get; set; }

        public static GetOrderOrderItemsQuery Create(string orderId)
        {
            return new GetOrderOrderItemsQuery
            {
                OrderId = orderId
            };
        }
    }
}