using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.Postgres.Domain.Entities.Associations
{
    public class N_CompositeTwo
    {
        public Guid Id { get; set; }

        public string CompositeTwoAttr { get; set; }
    }
}