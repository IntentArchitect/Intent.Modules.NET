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