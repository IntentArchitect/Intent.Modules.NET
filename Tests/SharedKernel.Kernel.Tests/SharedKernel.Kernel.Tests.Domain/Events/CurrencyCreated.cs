using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using SharedKernel.Kernel.Tests.Domain.Common;
using SharedKernel.Kernel.Tests.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.DomainEvents.DomainEvent", Version = "1.0")]

namespace SharedKernel.Kernel.Tests.Domain.Events
{
    public class CurrencyCreated : DomainEvent
    {
        public CurrencyCreated(Currency currency)
        {
            Currency = currency;
        }

        public Currency Currency { get; }
    }
}