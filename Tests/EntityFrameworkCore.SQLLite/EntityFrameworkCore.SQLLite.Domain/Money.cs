using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ValueObjects.ValueObject", Version = "1.0")]

namespace EntityFrameworkCore.SQLLite.Domain
{
    public class Money : ValueObject
    {
        public Money(decimal value, string currency)
        {
            Value = value;
            Currency = currency;
        }

        [IntentMerge]
        protected Money()
        {
            Currency = null!;
        }

        public decimal Value { get; private set; }
        public string Currency { get; private set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            // Using a yield return statement to return each element one at a time
            yield return Value;
            yield return Currency;
        }
    }
}