using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SQLLite.Domain.Entities
{
    public class Order
    {
        public Order()
        {
            RefNo = null!;
        }

        public Guid Id { get; set; }

        public string RefNo { get; set; }
    }
}