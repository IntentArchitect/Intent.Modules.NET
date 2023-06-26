using Intent.RoslynWeaver.Attributes;
using Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.DomainMapping.MessageExtensions", Version = "1.0")]

namespace MassTransit.Messages.Shared
{
    public static class BasketDeletedEventExtensions
    {
        public static BasketDeletedEvent MapToBasketDeletedEvent(this Basket projectFrom)
        {
            return new BasketDeletedEvent
            {
                Id = projectFrom.Id,
                Number = projectFrom.Number,
            };
        }
    }
}