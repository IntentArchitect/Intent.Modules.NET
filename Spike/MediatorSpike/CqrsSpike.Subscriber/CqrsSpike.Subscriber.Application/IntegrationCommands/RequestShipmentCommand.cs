namespace CqrsSpike.Subscriber.Eventing.Messages
{
    public record RequestShipmentCommand
    {
        public Guid OrderId { get; init; }
    }
}