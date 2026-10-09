namespace CqrsSpike.Publisher.Eventing.Messages
{
    public record OrderCreatedEvent
    {
        public OrderCreatedEvent()
        {
            CustomerName = null!;
        }

        public Guid OrderId { get; init; }
        public string CustomerName { get; init; }
        public decimal Total { get; init; }
    }
}