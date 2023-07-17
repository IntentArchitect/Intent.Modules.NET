using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.EntityFrameworkCore.BasicAuditing.AuditableInterface", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Fully)]

namespace EntityFrameworkCore.SqlServer.TestApplication.Domain.Common.Interfaces;

public interface IAuditable
{
    string CreatedBy { get; set; }
    DateTimeOffset CreatedDate { get; set; }
    string? UpdatedBy { get; set; }
    DateTimeOffset? UpdatedDate { get; set; }
}