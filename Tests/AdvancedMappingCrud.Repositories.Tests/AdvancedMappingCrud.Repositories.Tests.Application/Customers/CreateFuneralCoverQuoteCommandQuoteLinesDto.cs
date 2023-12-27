using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Customers
{
    public class CreateFuneralCoverQuoteCommandQuoteLinesDto
    {
        public CreateFuneralCoverQuoteCommandQuoteLinesDto()
        {
        }

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