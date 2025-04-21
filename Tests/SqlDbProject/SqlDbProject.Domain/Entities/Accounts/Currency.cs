using Intent.RoslynWeaver.Attributes;
using SqlDbProject.Domain.Common;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace SqlDbProject.Domain.Entities.Accounts
{
    public class Currency : IHasDomainEvent
    {
        public Currency()
        {
            Country = null!;
            Description = null!;
            AlphaCode = null!;
        }

        public int CurrencyIso { get; set; }

        public string Country { get; set; }

        public string Description { get; set; }

        public string AlphaCode { get; set; }

        public decimal NumericCode { get; set; }

        public bool IsEnabled { get; set; }

        public int? Sequence { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}