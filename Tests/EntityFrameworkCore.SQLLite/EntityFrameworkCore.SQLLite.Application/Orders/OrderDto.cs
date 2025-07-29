using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace EntityFrameworkCore.SQLLite.Application.Orders
{
    public class OrderDto
    {
        public OrderDto()
        {
            RefNo = null!;
        }

        public Guid Id { get; set; }
        public string RefNo { get; set; }

        public static OrderDto Create(Guid id, string refNo)
        {
            return new OrderDto
            {
                Id = id,
                RefNo = refNo
            };
        }
    }
}