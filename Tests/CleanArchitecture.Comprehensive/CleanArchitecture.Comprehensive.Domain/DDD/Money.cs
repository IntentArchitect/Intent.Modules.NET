using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ValueObjects.ValueObject", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Domain.DDD
{
    public class Money : ValueObject
    {
        protected Money()
        {
        }
        public Money(string currency, decimal amount)
        {
            Currency = currency;
            Amount = amount;
        }

        public string Currency { get; private set; }
        public decimal Amount { get; private set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            // Using a yield return statement to return each element one at a time
            yield return Currency;
            yield return Amount;
        }
    }
}