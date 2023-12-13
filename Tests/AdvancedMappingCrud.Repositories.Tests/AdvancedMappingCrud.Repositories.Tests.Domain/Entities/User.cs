using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Domain.Entities
{
    public class User : Person
    {
        public string Email { get; set; }

        public Guid QuoteId { get; set; }

        public virtual Quote Quote { get; set; }
    }
}