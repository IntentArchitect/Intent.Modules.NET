using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Customers
{
    public class ApproveQuoteCommand
    {
        public Guid QuoteId { get; set; }

        public static ApproveQuoteCommand Create(Guid quoteId)
        {
            return new ApproveQuoteCommand
            {
                QuoteId = quoteId
            };
        }
    }
}