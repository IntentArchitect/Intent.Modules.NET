using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CqrsSpike.Publisher.Application.AuditEntries
{
    public record AuditEntryDto
    {
        public AuditEntryDto()
        {
            Description = null!;
        }

        public Guid Id { get; init; }
        public string Description { get; init; }
        public DateTimeOffset CreatedAt { get; init; }
    }
}