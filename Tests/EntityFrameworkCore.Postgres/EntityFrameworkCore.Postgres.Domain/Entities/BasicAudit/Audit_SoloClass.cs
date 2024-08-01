using System;
using EntityFrameworkCore.Postgres.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.Postgres.Domain.Entities.BasicAudit
{
    public class Audit_SoloClass : IAuditable
    {
        public Guid Id { get; set; }

        public string SoloAttr { get; set; }

        public string CreatedBy { get; set; }

        public DateTimeOffset CreatedDate { get; set; }

        public string? UpdatedBy { get; set; }

        public DateTimeOffset? UpdatedDate { get; set; }

        void IAuditable.SetCreated(string createdBy, DateTimeOffset createdDate) => (CreatedBy, CreatedDate) = (createdBy, createdDate);

        void IAuditable.SetUpdated(string updatedBy, DateTimeOffset updatedDate) => (UpdatedBy, UpdatedDate) = (updatedBy, updatedDate);
    }
}