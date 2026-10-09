namespace CqrsSpike.Subscriber.Eventing.Messages
{
    public record ShipmentCreatedEvent
    {
        public Guid ShipmentId { get; init; }
        public Guid OrderId { get; init; }
    }
}