using System;
using System.Collections.Generic;
using CleanArchitecture.TestApplication.Domain.Common;
using CleanArchitecture.TestApplication.Domain.Services.DDD;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTagModeImplicit]

namespace CleanArchitecture.TestApplication.Domain.Entities.DDD
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
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

        }

        public void ChangeName(string name)
        {
            Name = name;
        }
    }
}