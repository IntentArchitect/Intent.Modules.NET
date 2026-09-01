using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace WolverineEventing.Coexist.Cqrs.Domain.Entities
{
    public class Order
    {
        public Order()
        {
            Status = null!;
        }

        public Guid Id { get; set; }

        public string Status { get; set; }
    }
}