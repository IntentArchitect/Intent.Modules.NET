using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.BasicAuditing.AuditableInterface", Version = "1.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Domain.Common.Interfaces
{
    public interface IAuditable
    {
        string? CreatedBy { get; set; }
        DateTimeOffset CreatedDate { get; set; }
        string? UpdatedBy { get; set; }
        DateTimeOffset? UpdatedDate { get; set; }
    }
}