namespace CqrsSpike.Subscriber.Domain.Entities
{
    public class OrderRecord
    {
        public OrderRecord()
        {
            CustomerName = null!;
        }

        public Guid Id { get; set; }

        public string CustomerName { get; set; }

        public decimal Total { get; set; }

        public Guid OrderId { get; set; }
    }
}