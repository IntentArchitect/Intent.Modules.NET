using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Services.Orders
{
    public class GetOrderOrderItemsQuery
    {
        public Guid OrderId { get; set; }

        public static GetOrderOrderItemsQuery Create(Guid orderId)
        {
            return new GetOrderOrderItemsQuery
            {
                OrderId = orderId
            };
        }
    }
}