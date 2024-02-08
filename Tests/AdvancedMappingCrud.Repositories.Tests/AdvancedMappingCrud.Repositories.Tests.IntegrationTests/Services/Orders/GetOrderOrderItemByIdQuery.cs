using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Orders
{
    public class GetOrderOrderItemByIdQuery
    {
        public Guid OrderId { get; set; }
        public Guid Id { get; set; }

        public static GetOrderOrderItemByIdQuery Create(Guid orderId, Guid id)
        {
            return new GetOrderOrderItemByIdQuery
            {
                OrderId = orderId,
                Id = id
            };
        }
    }
}