using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using RichDomain.Domain.Common;
using RichDomain.Domain.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace RichDomain.Domain.Entities
{
    public partial class EntityWithAutoAppliedNewFields : IEntityWithAutoAppliedNewFields, IHasDomainEvent
    {
        public Guid Id { get; private set; }

        public string CreatedByName { get; private set; }

        public DateTimeOffset CreatedDate { get; private set; }

        public string? UpdatedBy { get; private set; }

        public DateTimeOffset? UpdatedDate { get; private set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];

        void IAuditable.SetCreated(string createdBy, DateTimeOffset createdDate) => (CreatedByName, CreatedDate) = (createdBy, createdDate);

        void IAuditable.SetUpdated(string updatedBy, DateTimeOffset updatedDate) => (UpdatedBy, UpdatedDate) = (updatedBy, updatedDate);
    }
}