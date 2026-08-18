using Intent.RoslynWeaver.Attributes;
using ObjectMapping.Lenient.Domain;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace ObjectMapping.Lenient.Application.Orders
{
    public record CouponDto
    {
        public CouponDto()
        {
            Code = null!;
        }

        public Guid Id { get; init; }
        public string Code { get; init; }
        public int PercentOff { get; init; }
        public CouponKind Kind { get; init; }
    }
}