using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ValueObjects.ValueObject", Version = "1.0")]

namespace FastEndpointsTest.Domain.DDD
{
    public class Money : ValueObject
    {
        public Money(string currency, decimal amount)
        {
            Currency = currency;
            Amount = amount;
        }

        [IntentMerge]
        protected Money()
        {
            Currency = null!;
        }

        public string Currency { get; private set; }
        public decimal Amount { get; private set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Currency;
            yield return Amount;
        }
    }
}