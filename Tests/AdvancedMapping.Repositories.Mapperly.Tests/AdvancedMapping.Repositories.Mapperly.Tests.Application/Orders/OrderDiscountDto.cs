using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.Orders
{
    public class OrderDiscountDto
    {
        public OrderDiscountDto()
        {
            Description = null!;
        }

        public Guid Id { get; set; }
        public string Description { get; set; }
        public double? Percentage { get; set; }
        public decimal? FixedAmount { get; set; }

        public static OrderDiscountDto Create(Guid id, string description, double? percentage, decimal? fixedAmount)
        {
            return new OrderDiscountDto
            {
                Id = id,
                Description = description,
                Percentage = percentage,
                FixedAmount = fixedAmount
            };
        }
    }
}