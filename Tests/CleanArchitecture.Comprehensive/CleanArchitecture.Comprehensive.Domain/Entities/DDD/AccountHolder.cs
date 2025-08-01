using System;
using System.Collections.Generic;
using CleanArchitecture.Comprehensive.Domain.Common;
using CleanArchitecture.Comprehensive.Domain.Services.DDD;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Domain.Entities.DDD
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

        public virtual ICollection<Account> Accounts { get; set; } = [];

        public List<DomainEvent> DomainEvents { get; set; } = [];

        public void Transfer(string description, IAccountingDomainService service, decimal amount, string currency)
        {

        }

        public void ChangeName(string name)
        {
            Name = name;
        }
    }
}