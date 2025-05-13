using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Intent.Modules.NET.Tests.Module1.Application.Contracts.Orders
{
    public class OrderUpdateDto
    {
        public OrderUpdateDto()
        {
            RefNo = null!;
        }

        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public string RefNo { get; set; }

        public static OrderUpdateDto Create(Guid id, Guid customerId, string refNo)
        {
            return new OrderUpdateDto
            {
                Id = id,
                CustomerId = customerId,
                RefNo = refNo
            };
        }
    }
}