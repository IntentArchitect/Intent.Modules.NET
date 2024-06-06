using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace ProxyServiceTests.OriginalServices.Domain.Entities
{
    public class Account
    {
        public Guid Id { get; set; }

        public string Number { get; set; }

        public Money Amount { get; set; }

        public Guid ClientId { get; set; }

        public virtual Client Client { get; set; }
    }
}