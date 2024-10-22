using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using SharedKernel.Consumer.Tests.Domain.Common;
using SharedKernel.Consumer.Tests.Domain.Events;
using SharedKernel.Kernel.Tests.Domain.Entities;
using SharedKernel.Kernel.Tests.Domain.Services;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace SharedKernel.Consumer.Tests.Domain.Entities
{
    public class Order : IHasDomainEvent
    {
        public Order(string refNo, Guid countryId, ICurrencyService service)
        {
            RefNo = refNo;
            CountryId = countryId;
            Currency = service.GetDefaultCurrency(countryId);
            DomainEvents.Add(new OrderCreated(order: this));
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        protected Order()
        {
            RefNo = null!;
            Country = null!;
            Currency = null!;
        }

        public Guid Id { get; set; }

        public string RefNo { get; set; }

        public Guid CountryId { get; set; }

        public Guid CurrencyId { get; set; }

        public virtual Country Country { get; set; }

        public virtual Currency Currency { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}