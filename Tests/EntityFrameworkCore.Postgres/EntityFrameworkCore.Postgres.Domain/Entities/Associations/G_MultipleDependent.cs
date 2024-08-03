using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.Postgres.Domain.Entities.Associations
{
    public class G_MultipleDependent
    {
        public Guid Id { get; set; }

        public string MultipleDepAttr { get; set; }

        public Guid G_RequiredCompositeNavId { get; set; }

        public virtual G_RequiredCompositeNav G_RequiredCompositeNav { get; set; }
    }
}