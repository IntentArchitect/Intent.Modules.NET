using Intent.RoslynWeaver.Attributes;
using Publish.CleanArch.MassTransit.OutboxEF.TestApplication.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.DomainMapping.DtoExtensions", Version = "1.0")]

namespace MassTransit.Messages.Shared
{
    public static class OrderItemDtoExtensions
    {
        public static OrderItemDto MapToOrderItemDto(this OrderItem projectFrom)
        {
            return new OrderItemDto
            {
                Id = projectFrom.Id,
                OrderId = projectFrom.OrderId,
                Description = projectFrom.Description,
                Amount = projectFrom.Amount,
            };
        }
    }
}