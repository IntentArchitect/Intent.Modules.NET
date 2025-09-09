using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.PkNoneProvider.Domain.Entities.Accounts.NotSchema
{
    public class AccTableFolder
    {
        public Guid Id { get; set; }
    }
}