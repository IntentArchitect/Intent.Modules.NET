using System;
using System.Collections.Generic;
using BasicAuditing.CustomUserId.Tests.Domain.Common;
using BasicAuditing.CustomUserId.Tests.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace BasicAuditing.CustomUserId.Tests.Domain.Entities
{
    public class Customer : IHasDomainEvent, IAuditable
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid CreatedBy { get; set; }

        public DateTimeOffset CreatedDate { get; set; }

        public Guid? UpdatedBy { get; set; }

        public DateTimeOffset? UpdatedDate { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();

        void IAuditable.SetCreated(Guid createdBy, DateTimeOffset createdDate) => (CreatedBy, CreatedDate) = (createdBy, createdDate);

        void IAuditable.SetUpdated(Guid updatedBy, DateTimeOffset updatedDate) => (UpdatedBy, UpdatedDate) = (updatedBy, updatedDate);
    }
}