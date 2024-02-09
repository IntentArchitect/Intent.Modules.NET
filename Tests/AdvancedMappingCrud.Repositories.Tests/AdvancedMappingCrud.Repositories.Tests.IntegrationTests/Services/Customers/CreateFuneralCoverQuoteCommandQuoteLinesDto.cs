using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Customers
{
    public class CreateFuneralCoverQuoteCommandQuoteLinesDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }

        public static CreateFuneralCoverQuoteCommandQuoteLinesDto Create(Guid id, Guid productId)
        {
            return new CreateFuneralCoverQuoteCommandQuoteLinesDto
            {
                Id = id,
                ProductId = productId
            };
        }
    }
}