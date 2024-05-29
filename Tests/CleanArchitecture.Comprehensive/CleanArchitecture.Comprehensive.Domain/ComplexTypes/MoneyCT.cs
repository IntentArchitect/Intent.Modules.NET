using System.Collections.Generic;
using CleanArchitecture.Comprehensive.Domain;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ValueObjects.ValueObject", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Domain.ComplexTypes
{
    public class MoneyCT : ValueObject
    {
        protected MoneyCT()
        {
        }

        public MoneyCT(decimal amount, string currency)
        {
            Amount = amount;
            Currency = currency;
        }

        public decimal Amount { get; private set; }
        public string Currency { get; private set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            // Using a yield return statement to return each element one at a time
            yield return Amount;
            yield return Currency;
        }
    }
}