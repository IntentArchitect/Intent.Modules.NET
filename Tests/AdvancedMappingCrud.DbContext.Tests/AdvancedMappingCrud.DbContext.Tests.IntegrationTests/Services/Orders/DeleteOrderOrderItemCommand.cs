using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.DbContext.Tests.IntegrationTests.Services.Orders
{
    public class DeleteOrderOrderItemCommand
    {
        public Guid OrderId { get; set; }
        public Guid Id { get; set; }

        public static DeleteOrderOrderItemCommand Create(Guid orderId, Guid id)
        {
            return new DeleteOrderOrderItemCommand
            {
                OrderId = orderId,
                Id = id
            };
        }
    }
}