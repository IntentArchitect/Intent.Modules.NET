using System;
using Intent.RoslynWeaver.Attributes;
using RichDomain.Domain.Common;
using RichDomain.Domain.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace RichDomain.Domain.Entities
{
    public interface IEntityWithAutoAppliedNewFields : IHasDomainEvent, IAuditable
    {
        Guid Id { get; }

        string CreatedByName { get; }

        DateTimeOffset CreatedDate { get; }

        string? UpdatedBy { get; }

        DateTimeOffset? UpdatedDate { get; }
    }
}