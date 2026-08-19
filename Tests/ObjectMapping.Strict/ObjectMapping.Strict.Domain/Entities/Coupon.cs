using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace ObjectMapping.Strict.Domain.Entities
{
    public class Coupon
    {
        public Coupon()
        {
            Code = null!;
        }

        public Guid Id { get; set; }

        public string Code { get; set; }

        public int PercentOff { get; set; }

        public CouponKind Kind { get; set; }
    }
}