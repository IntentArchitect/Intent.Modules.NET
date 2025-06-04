using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AspNetCoreCleanArchitecture.Sample.Application.Orders
{
    public class UpdateOrderCommandOrderLinesDto
    {
        public UpdateOrderCommandOrderLinesDto()
        {
        }

        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public int Units { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal? Discount { get; set; }

        public static UpdateOrderCommandOrderLinesDto Create(
            Guid id,
            Guid productId,
            int units,
            decimal unitPrice,
            decimal? discount)
        {
            return new UpdateOrderCommandOrderLinesDto
            {
                Id = id,
                ProductId = productId,
                Units = units,
                UnitPrice = unitPrice,
                Discount = discount
            };
        }
    }
}