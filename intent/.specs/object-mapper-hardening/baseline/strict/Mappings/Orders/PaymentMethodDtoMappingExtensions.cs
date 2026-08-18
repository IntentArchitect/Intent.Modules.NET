using Intent.RoslynWeaver.Attributes;
using ObjectMapping.Strict.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.ObjectMapping.MappingExtensions", Version = "1.0")]

namespace ObjectMapping.Strict.Application.Orders
{
    public static class PaymentMethodDtoMappingExtensions
    {
        public static PaymentMethodDto MapToPaymentMethodDto(this PaymentMethod projectFrom)
        {
            return new PaymentMethodDto
            {
                Id = projectFrom.Id,
                Label = projectFrom.Label,
            };
        }

        public static List<PaymentMethodDto> MapToPaymentMethodDtoList(this IEnumerable<PaymentMethod> projectFrom) => projectFrom.Select(x => x.MapToPaymentMethodDto()).ToList();
    }
}
