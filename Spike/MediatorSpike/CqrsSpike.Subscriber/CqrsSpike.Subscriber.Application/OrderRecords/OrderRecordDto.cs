namespace CqrsSpike.Subscriber.Application.OrderRecords
{
    public record OrderRecordDto
    {
        public OrderRecordDto()
        {
            CustomerName = null!;
        }

        public Guid Id { get; init; }
        public string CustomerName { get; init; }
        public decimal Total { get; init; }
        public Guid OrderId { get; init; }
    }
}