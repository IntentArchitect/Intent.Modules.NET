using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Domain.Entities
{
    public class DispatchDocument
    {
        public DispatchDocument()
        {
            DocumentNumber = null!;
        }

        public Guid Id { get; set; }

        public string DocumentNumber { get; set; }

        public DateTime IssuedOn { get; set; }

        public string? FileUrl { get; set; }
    }
}