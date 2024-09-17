using System;
using System.Collections.Generic;
using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Entities
{
    public class Customer : IHasDomainEvent
    {
        public Customer(User user, string login)
        {
            Login = login;
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        protected Customer()
        {
            Login = null!;
            User = null!;
        }

        public Guid Id { get; set; }

        public string Login { get; set; }

        public Guid UserId { get; set; }

        public virtual User User { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}