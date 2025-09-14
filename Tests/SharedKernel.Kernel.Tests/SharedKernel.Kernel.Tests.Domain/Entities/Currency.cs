using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using SharedKernel.Kernel.Tests.Domain.Common;
using SharedKernel.Kernel.Tests.Domain.Events;

namespace SharedKernel.Kernel.Tests.Domain.Entities
{
    public class Currency : IHasDomainEvent
    {
        public Currency(Guid countryId, string name, string symbol)
        {
            CountryId = countryId;
            Name = name;
            Symbol = symbol;
            DomainEvents.Add(new CurrencyCreated(
                currency: this));
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        protected Currency()
        {
            Name = null!;
            Symbol = null!;
            Description = null!;
            Country = null!;
        }

        public Guid Id { get; set; }

        public Guid CountryId { get; set; }

        public string Name { get; set; }

        public string Symbol { get; set; }

        public string Description { get; set; }

        public virtual Country Country { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}