using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.EF8.Domain.Entities.Associations
{
    public class O_DestNameDiffDependent
    {
        public Guid Id { get; set; }

        public Guid ODestnamediffId { get; set; }
    }
}