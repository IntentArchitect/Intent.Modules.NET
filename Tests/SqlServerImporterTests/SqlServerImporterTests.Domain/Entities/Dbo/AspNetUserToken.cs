using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using SqlServerImporterTests.Domain.Common;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace SqlServerImporterTests.Domain.Entities.Dbo
{
    public class AspNetUserToken : IHasDomainEvent
    {
        public string UserId { get; set; }

        public string LoginProvider { get; set; }

        public string Name { get; set; }

        public string? Value { get; set; }

        public virtual AspNetUser UserIdAspNetUsers { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}