using System;
using System.Collections.Generic;
using AdvancedMappingCrud.Repositories.Tests.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Domain.Entities.AnemicChild
{
    public class ParentWithAnemicChild : IHasDomainEvent
    {
        public ParentWithAnemicChild(string name, string surname, IEnumerable<AnemicChild> anemicChildren)
        {
            Name = name;
            Surname = surname;
            AnemicChildren = new List<AnemicChild>(anemicChildren);
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        protected ParentWithAnemicChild()
        {
            Name = null!;
            Surname = null!;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public virtual ICollection<AnemicChild> AnemicChildren { get; set; } = [];

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}