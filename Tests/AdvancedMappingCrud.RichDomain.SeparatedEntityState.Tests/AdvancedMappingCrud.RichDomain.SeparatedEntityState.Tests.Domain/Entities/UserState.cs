using System;
using System.Collections.Generic;
using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Entities
{
    public partial class User : IHasDomainEvent
    {
        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        [IntentManaged(Mode.Fully)]
        protected User()
        {
            Company = null!;
            ContactDetails = null!;
        }

        public Guid Id { get; set; }

        public Guid CompanyId { get; set; }

        public virtual Company Company { get; set; }

        public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();

        public ContactDetailsVO ContactDetails { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}