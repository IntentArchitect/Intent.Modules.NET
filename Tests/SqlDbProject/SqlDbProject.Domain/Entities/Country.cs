using Intent.RoslynWeaver.Attributes;
using SqlDbProject.Domain.Common;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace SqlDbProject.Domain.Entities
{
    public class Country : IHasDomainEvent
    {
        public Country()
        {
            CountryIso = null!;
            Description = null!;
            CurrencyIso = null!;
            DialingCode = null!;
        }

        public string CountryIso { get; set; }

        public string Description { get; set; }

        public string CurrencyIso { get; set; }

        public string DialingCode { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}