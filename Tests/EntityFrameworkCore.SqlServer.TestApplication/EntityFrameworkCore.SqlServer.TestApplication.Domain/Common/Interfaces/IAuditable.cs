using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.EntityFrameworkCore.BasicAuditing.AuditableInterface", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Fully)]

namespace EntityFrameworkCore.SqlServer.TestApplication.Domain.Common.Interfaces;

public interface IAuditable
{
    void SetCreated(string createdBy, DateTimeOffset createdDate);
    void SetUpdated(string updatedBy, DateTimeOffset updatedDate);
}