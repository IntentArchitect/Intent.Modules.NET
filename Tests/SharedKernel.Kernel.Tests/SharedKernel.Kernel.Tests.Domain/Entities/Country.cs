using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using SharedKernel.Kernel.Tests.Domain.Common;
using SharedKernel.Kernel.Tests.Domain.Events;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace SharedKernel.Kernel.Tests.Domain.Entities
{
    public class Country : IHasDomainEvent
    {
        public Country(string name, string code)
        {
            Name = name;
            Code = code;
            DomainEvents.Add(new CountryCreated(country: this));
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        protected Country()
        {
            Name = null!;
            Code = null!;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}