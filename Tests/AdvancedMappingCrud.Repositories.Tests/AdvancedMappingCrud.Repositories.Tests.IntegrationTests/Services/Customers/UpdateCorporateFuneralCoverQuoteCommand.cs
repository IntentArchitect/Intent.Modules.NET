using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Customers
{
    public class UpdateCorporateFuneralCoverQuoteCommand
    {
        public UpdateCorporateFuneralCoverQuoteCommand()
        {
            RefNo = null!;
            QuoteLines = null!;
            Corporate = null!;
            Registration = null!;
        }

        public string RefNo { get; set; }
        public Guid PersonId { get; set; }
        public string? PersonEmail { get; set; }
        public List<CreateFuneralCoverQuoteCommandQuoteLinesDto> QuoteLines { get; set; }
        public string Corporate { get; set; }
        public string Registration { get; set; }
        public Guid Id { get; set; }
        public decimal Amount { get; set; }

        public static UpdateCorporateFuneralCoverQuoteCommand Create(
            string refNo,
            Guid personId,
            string? personEmail,
            List<CreateFuneralCoverQuoteCommandQuoteLinesDto> quoteLines,
            string corporate,
            string registration,
            Guid id,
            decimal amount)
        {
            return new UpdateCorporateFuneralCoverQuoteCommand
            {
                RefNo = refNo,
                PersonId = personId,
                PersonEmail = personEmail,
                QuoteLines = quoteLines,
                Corporate = corporate,
                Registration = registration,
                Id = id,
                Amount = amount
            };
        }
    }
}