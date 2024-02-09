using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.Services.Orders
{
    public class GetOrderOrderItemByIdQuery
    {
        public GetOrderOrderItemByIdQuery()
        {
            OrderId = null!;
            Id = null!;
        }

        public string OrderId { get; set; }
        public string Id { get; set; }

        public static GetOrderOrderItemByIdQuery Create(string orderId, string id)
        {
            return new GetOrderOrderItemByIdQuery
            {
                OrderId = orderId,
                Id = id
            };
        }
    }
}