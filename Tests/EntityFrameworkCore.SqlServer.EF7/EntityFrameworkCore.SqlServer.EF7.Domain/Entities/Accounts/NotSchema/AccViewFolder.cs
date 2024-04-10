using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.EF7.Domain.Entities.Accounts.NotSchema
{
    public class AccViewFolder
    {
        public Guid Id { get; set; }
    }
}