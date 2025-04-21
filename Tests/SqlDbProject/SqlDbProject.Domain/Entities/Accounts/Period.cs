using Intent.RoslynWeaver.Attributes;
using SqlDbProject.Domain.Common;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace SqlDbProject.Domain.Entities.Accounts
{
    public class Period : IHasDomainEvent
    {
        public Period()
        {
            Description = null!;
        }

        public int PeriodId { get; set; }

        public string Description { get; set; }

        public DateOnly StartDate { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}