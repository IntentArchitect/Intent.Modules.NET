using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AspNetCoreCleanArchitecture.Sample.Application.Orders
{
    public class CreateOrderCommandOrderLinesDto
    {
        public CreateOrderCommandOrderLinesDto()
        {
        }

        public Guid ProductId { get; set; }
        public int Units { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal? Discount { get; set; }

        public static CreateOrderCommandOrderLinesDto Create(Guid productId, int units, decimal unitPrice, decimal? discount)
        {
            return new CreateOrderCommandOrderLinesDto
            {
                ProductId = productId,
                Units = units,
                UnitPrice = unitPrice,
                Discount = discount
            };
        }
    }
}