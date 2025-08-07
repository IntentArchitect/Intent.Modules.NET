using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ValueObjects.ValueObject", Version = "1.0")]

namespace EntityFrameworkCore.SQLLite.Domain
{
    public class OrderLine : ValueObject
    {
        public OrderLine(int units, Money price, Money? discount)
        {
            Units = units;
            Price = price;
            Discount = discount;
        }

        [IntentMerge]
        protected OrderLine()
        {
            Price = null!;
        }

        public int Units { get; private set; }
        public Money Price { get; private set; }
        public Money? Discount { get; private set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            // Using a yield return statement to return each element one at a time
            yield return Units;
            yield return Price;
            yield return Discount;
        }
    }
}