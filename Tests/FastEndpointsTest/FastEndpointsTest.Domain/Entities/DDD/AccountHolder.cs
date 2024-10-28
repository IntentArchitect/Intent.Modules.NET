using System;
using System.Collections.Generic;
using FastEndpointsTest.Domain.Common;
using FastEndpointsTest.Domain.Services.DDD;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace FastEndpointsTest.Domain.Entities.DDD
{
    public class AccountHolder : IHasDomainEvent
    {
        public AccountHolder(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        protected AccountHolder()
        {
            Name = null!;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();

        public void Transfer(string description, IAccountingDomainService service, decimal amount, string currency)
        {
            // [IntentFully]
            // TODO: Implement Transfer (AccountHolder) functionality
            throw new NotImplementedException("Replace with your implementation...");
        }

        public void ChangeName(string name)
        {
            Name = name;
        }
    }
}