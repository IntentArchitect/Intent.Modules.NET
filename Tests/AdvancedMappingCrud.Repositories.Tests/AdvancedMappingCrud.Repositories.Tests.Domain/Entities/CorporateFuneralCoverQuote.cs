using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Domain.Entities
{
    public class CorporateFuneralCoverQuote : FuneralCoverQuote
    {
        public CorporateFuneralCoverQuote(string refNo, Guid personId, string? personEmail)
        {
            base.RefNo = refNo;
            base.PersonId = personId;
            base.PersonEmail = personEmail;
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        protected CorporateFuneralCoverQuote()
        {
            Corporate = null!;
            Registration = null!;
        }
        public string Corporate { get; set; }

        public string Registration { get; set; }
    }
}