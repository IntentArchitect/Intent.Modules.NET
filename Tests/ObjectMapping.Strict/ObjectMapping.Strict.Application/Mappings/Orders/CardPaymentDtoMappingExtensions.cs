using Intent.RoslynWeaver.Attributes;
using ObjectMapping.Strict.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.ObjectMapping.MappingExtensions", Version = "1.0")]

namespace ObjectMapping.Strict.Application.Orders
{
    public static class CardPaymentDtoMappingExtensions
    {
        public static CardPaymentDto MapToCardPaymentDto(this CardPayment projectFrom)
        {
            return new CardPaymentDto
            {
                Id = projectFrom.Id,
                Label = projectFrom.Label,
                CardLast4 = projectFrom.CardLast4
            };
        }

        public static List<CardPaymentDto> MapToCardPaymentDtoList(this IEnumerable<CardPayment> projectFrom) => projectFrom.Select(x => x.MapToCardPaymentDto()).ToList();
    }
}