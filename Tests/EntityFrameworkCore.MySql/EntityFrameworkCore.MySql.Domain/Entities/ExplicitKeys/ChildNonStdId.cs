using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.MySql.Domain.Entities.ExplicitKeys
{
    public class ChildNonStdId
    {
        public Guid DiffId { get; set; }

        public string Name { get; set; }
    }
}