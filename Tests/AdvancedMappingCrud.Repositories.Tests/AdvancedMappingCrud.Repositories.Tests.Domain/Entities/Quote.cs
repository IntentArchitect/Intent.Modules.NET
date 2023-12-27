using System;
using System.Collections.Generic;
using AdvancedMappingCrud.Repositories.Tests.Domain.Common;
using AdvancedMappingCrud.Repositories.Tests.Domain.Events;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Domain.Entities
{
    public class Quote : IHasDomainEvent
    {
        public Quote(string refNo, Guid personId, string? personEmail)
        {
            RefNo = refNo;
            PersonId = personId;
            PersonEmail = personEmail;
            DomainEvents.Add(new NewQuoteCreated(quote: this));
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        protected Quote()
        {
            RefNo = null!;
        }

        public Guid Id { get; set; }

        public string RefNo { get; set; }

        public Guid PersonId { get; set; }

        public string? PersonEmail { get; set; }

        public virtual ICollection<QuoteLine> QuoteLines { get; set; } = new List<QuoteLine>();

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();

        public void NotifyQuoteCreated()
        {
            DomainEvents.Add(new NewQuoteCreated(quote: this));
        }
    }
}