using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Customers
{
    public class CreateCorporateFuneralCoverQuoteCommand
    {
        public CreateCorporateFuneralCoverQuoteCommand()
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

        public static CreateCorporateFuneralCoverQuoteCommand Create(
            string refNo,
            Guid personId,
            string? personEmail,
            List<CreateFuneralCoverQuoteCommandQuoteLinesDto> quoteLines,
            string corporate,
            string registration)
        {
            return new CreateCorporateFuneralCoverQuoteCommand
            {
                RefNo = refNo,
                PersonId = personId,
                PersonEmail = personEmail,
                QuoteLines = quoteLines,
                Corporate = corporate,
                Registration = registration
            };
        }
    }
}