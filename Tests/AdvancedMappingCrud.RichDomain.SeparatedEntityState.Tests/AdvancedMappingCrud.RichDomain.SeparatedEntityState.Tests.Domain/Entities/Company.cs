using System;
using System.Collections.Generic;
using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Entities
{
    public class Company : IHasDomainEvent
    {
        public Company(string name, IEnumerable<ContactDetailsVO> contactDetailsVOS)
        {
            Name = name;
            ContactDetailsVOS = new List<ContactDetailsVO>(contactDetailsVOS);
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        protected Company()
        {
            Name = null!;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public ICollection<ContactDetailsVO> ContactDetailsVOS { get; set; } = new List<ContactDetailsVO>();

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}