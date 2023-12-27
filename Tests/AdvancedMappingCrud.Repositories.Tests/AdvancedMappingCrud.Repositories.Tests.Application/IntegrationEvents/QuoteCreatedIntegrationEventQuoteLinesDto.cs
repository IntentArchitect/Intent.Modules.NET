using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventDto", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Eventing.Messages
{
    public class QuoteCreatedIntegrationEventQuoteLinesDto
    {
        public QuoteCreatedIntegrationEventQuoteLinesDto()
        {
        }

        public Guid Id { get; set; }
        public Guid ProductId { get; set; }

        public static QuoteCreatedIntegrationEventQuoteLinesDto Create(Guid id, Guid productId)
        {
            return new QuoteCreatedIntegrationEventQuoteLinesDto
            {
                Id = id,
                ProductId = productId
            };
        }
    }
}