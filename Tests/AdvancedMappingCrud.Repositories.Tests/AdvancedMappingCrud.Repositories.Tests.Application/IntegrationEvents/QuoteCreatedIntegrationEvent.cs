using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventMessage", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Eventing.Messages
{
    public record QuoteCreatedIntegrationEvent
    {
        public QuoteCreatedIntegrationEvent()
        {
            RefNo = null!;
            QuoteLines = null!;
        }

        public Guid Id { get; init; }
        public string RefNo { get; init; }
        public Guid PersonId { get; init; }
        public string? PersonEmail { get; init; }
        public List<QuoteCreatedIntegrationEventQuoteLinesDto> QuoteLines { get; init; }
    }
}