using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AspNetCoreCleanArchitecture.Sample.Application.Orders
{
    public class OrderOrderOrderLineDto
    {
        public OrderOrderOrderLineDto()
        {
            ProductName = null!;
            ProductDescription = null!;
            ProductImageUrl = null!;
        }

        public Guid ProductId { get; set; }
        public int Units { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }
        public Guid Id { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public decimal ProductPrice { get; set; }
        public string ProductImageUrl { get; set; }

        public static OrderOrderOrderLineDto Create(
            Guid productId,
            int units,
            decimal unitPrice,
            decimal discount,
            Guid id,
            string productName,
            string productDescription,
            decimal productPrice,
            string productImageUrl)
        {
            return new OrderOrderOrderLineDto
            {
                ProductId = productId,
                Units = units,
                UnitPrice = unitPrice,
                Discount = discount,
                Id = id,
                ProductName = productName,
                ProductDescription = productDescription,
                ProductPrice = productPrice,
                ProductImageUrl = productImageUrl
            };
        }
    }
}