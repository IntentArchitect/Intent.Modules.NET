using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.MaintainColumnOrder.Tests.Domain.Entities
{
    public class Customer : Person
    {
        public Customer()
        {
            Col1 = null!;
            Col2 = null!;
        }
        public string Col1 { get; set; }

        public string Col2 { get; set; }
    }
}