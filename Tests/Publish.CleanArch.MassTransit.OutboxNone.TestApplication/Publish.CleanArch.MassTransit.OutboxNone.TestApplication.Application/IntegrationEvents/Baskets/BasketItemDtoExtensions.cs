using Intent.RoslynWeaver.Attributes;
using Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.DomainMapping.DtoExtensions", Version = "1.0")]

namespace MassTransit.Messages.Shared
{
    public static class BasketItemDtoExtensions
    {
        public static BasketItemDto MapToBasketItemDto(this BasketItem projectFrom)
        {
            return new BasketItemDto
            {
                Id = projectFrom.Id,
                Description = projectFrom.Description,
                Amount = projectFrom.Amount,
                BasketId = projectFrom.BasketId,
            };
        }
    }
}