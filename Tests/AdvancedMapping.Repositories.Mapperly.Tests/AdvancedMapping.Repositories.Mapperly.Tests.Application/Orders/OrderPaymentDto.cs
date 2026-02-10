using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.Orders
{
    public class OrderPaymentDto
    {
        public OrderPaymentDto()
        {
            Method = null!;
        }

        public Guid Id { get; set; }
        public string Method { get; set; }
        public decimal Amount { get; set; }
        public DateTime? PaidOn { get; set; }

        public static OrderPaymentDto Create(Guid id, string method, decimal amount, DateTime? paidOn)
        {
            return new OrderPaymentDto
            {
                Id = id,
                Method = method,
                Amount = amount,
                PaidOn = paidOn
            };
        }
    }
}