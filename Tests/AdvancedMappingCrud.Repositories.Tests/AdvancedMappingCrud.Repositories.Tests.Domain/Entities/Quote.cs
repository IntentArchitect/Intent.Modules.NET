using System;
using System.Collections.Generic;
using AdvancedMappingCrud.Repositories.Tests.Domain.Common;
using AdvancedMappingCrud.Repositories.Tests.Domain.Events;
using Intent.RoslynWeaver.Attributes;

namespace AdvancedMappingCrud.Repositories.Tests.Domain.Entities
{
    public class Quote : IHasDomainEvent
    {
        public Quote(string refNo, Guid personId, string? personEmail)
        {
            RefNo = refNo;
            PersonId = personId;
            PersonEmail = personEmail;
            DomainEvents.Add(new NewQuoteCreated(
                quote: this));
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

        public virtual ICollection<QuoteLine> QuoteLines { get; set; } = [];

        public List<DomainEvent> DomainEvents { get; set; } = [];

        public void NotifyQuoteCreated()
        {
            DomainEvents.Add(new NewQuoteCreated(
                quote: this));
        }
    }
}