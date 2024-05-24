using System;
using System.Collections.Generic;
using AdvancedMappingCrud.RichDomain.Tests.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Domain.Entities
{
    public class Customer : IHasDomainEvent
    {
        public Customer(User user, string login)
        {
            Login = login;
            User = user;
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        protected Customer()
        {
            Login = null!;
            User = null!;
        }

        public Guid Id { get; private set; }

        public string Login { get; private set; }

        public Guid UserId { get; private set; }

        public virtual User User { get; private set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}