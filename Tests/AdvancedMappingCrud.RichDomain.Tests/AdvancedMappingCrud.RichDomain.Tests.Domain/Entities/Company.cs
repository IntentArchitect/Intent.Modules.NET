using System;
using System.Collections.Generic;
using AdvancedMappingCrud.RichDomain.Tests.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Domain.Entities
{
    public class Company : IHasDomainEvent
    {
        private List<ContactDetailsVO> _contactDetailsVOS = new List<ContactDetailsVO>();
        public Company(string name, IEnumerable<ContactDetailsVO> contactDetailsVOS)
        {
            Name = name;
            _contactDetailsVOS = new List<ContactDetailsVO>(contactDetailsVOS);
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        protected Company()
        {
            Name = null!;
        }

        public Guid Id { get; private set; }

        public string Name { get; private set; }

        public IReadOnlyCollection<ContactDetailsVO> ContactDetailsVOS
        {
            get => _contactDetailsVOS.AsReadOnly();
            private set => _contactDetailsVOS = new List<ContactDetailsVO>(value);
        }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}