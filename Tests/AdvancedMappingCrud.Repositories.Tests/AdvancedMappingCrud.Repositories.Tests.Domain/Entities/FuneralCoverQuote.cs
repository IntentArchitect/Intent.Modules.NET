using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Domain.Entities
{
    public class FuneralCoverQuote : Quote
    {
        public FuneralCoverQuote(string refNo, Guid personId, string? personEmail)
        {
            RefNo = refNo;
            PersonId = personId;
            PersonEmail = personEmail;
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