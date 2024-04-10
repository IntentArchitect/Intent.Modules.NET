using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.BasicAuditing.AuditableInterface", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF7.Domain.Common.Interfaces
{
    public interface IAuditable
    {
        void SetCreated(string createdBy, DateTimeOffset createdDate);
        void SetUpdated(string updatedBy, DateTimeOffset updatedDate);
    }
}