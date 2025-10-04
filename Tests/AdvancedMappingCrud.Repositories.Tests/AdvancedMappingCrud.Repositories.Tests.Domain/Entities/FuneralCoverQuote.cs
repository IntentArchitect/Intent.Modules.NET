using System;
using AdvancedMappingCrud.Repositories.Tests.Domain.Events;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Domain.Entities
{
    public class FuneralCoverQuote : Quote
    {
        public FuneralCoverQuote(string refNo, Guid personId, string? personEmail) : base(refNo, personId, personEmail)
        {
            DomainEvents.Add(new NewQuoteCreated(
                quote: this));
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        protected FuneralCoverQuote()
        {
        }
        public decimal Amount { get; set; }
    }
}