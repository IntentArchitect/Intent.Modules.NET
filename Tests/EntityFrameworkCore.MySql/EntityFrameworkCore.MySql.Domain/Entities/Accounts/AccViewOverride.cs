using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.MySql.Domain.Entities.Accounts
{
    public class AccViewOverride
    {
        public Guid Id { get; set; }
    }
}