using System;
using System.Collections.Generic;
using AdvancedMappingCrud.Repositories.Tests.Domain.Common;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.DomainEvents.DomainEvent", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Domain.Events
{
    public class NewQuoteCreated : DomainEvent
    {
        public NewQuoteCreated(Quote quote)
        {
            Quote = quote;
        }

        public Quote Quote { get; }
    }
}