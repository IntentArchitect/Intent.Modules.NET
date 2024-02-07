using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Customers
{
    public class CreateFuneralCoverQuoteCommand
    {
        public CreateFuneralCoverQuoteCommand()
        {
            RefNo = null!;
            QuoteLines = null!;
        }

        public string RefNo { get; set; }
        public Guid PersonId { get; set; }
        public string? PersonEmail { get; set; }
        public List<CreateFuneralCoverQuoteCommandQuoteLinesDto> QuoteLines { get; set; }

        public static CreateFuneralCoverQuoteCommand Create(
            string refNo,
            Guid personId,
            string? personEmail,
            List<CreateFuneralCoverQuoteCommandQuoteLinesDto> quoteLines)
        {
            return new CreateFuneralCoverQuoteCommand
            {
                RefNo = refNo,
                PersonId = personId,
                PersonEmail = personEmail,
                QuoteLines = quoteLines
            };
        }
    }
}