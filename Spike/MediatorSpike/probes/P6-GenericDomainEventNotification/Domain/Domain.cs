namespace Probe.Domain;
public abstract class DomainEvent { public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow; }
public class OrderCreatedDomainEvent(Guid id) : DomainEvent { public Guid Id { get; } = id; }
public class OrderDeletedDomainEvent(Guid id) : DomainEvent { public Guid Id { get; } = id; }
