using Intent.RoslynWeaver.Attributes;
using ObjectMapping.Lenient.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.ObjectMapping.MappingExtensions", Version = "1.0")]

namespace ObjectMapping.Lenient.Application.Orders
{
    public static class CouponDtoMappingExtensions
    {
        public static CouponDto MapToCouponDto(this Coupon projectFrom)
        {
            return new CouponDto
            {
                Id = projectFrom.Id,
                Code = projectFrom.Code,
                PercentOff = projectFrom.PercentOff,
                Kind = projectFrom.Kind
            };
        }

        public static List<CouponDto> MapToCouponDtoList(this IEnumerable<Coupon> projectFrom) => projectFrom.Select(x => x.MapToCouponDto()).ToList();
    }
}