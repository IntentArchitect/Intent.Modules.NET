using System;
using System.Collections.Generic;
using AdvancedMappingCrud.RichDomain.Tests.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Domain.Entities
{
    public class Category : IHasDomainEvent
    {
        public Category(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        protected Category()
        {
            Name = null!;
        }
        public Guid Id { get; private set; }

        public string Name { get; private set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}