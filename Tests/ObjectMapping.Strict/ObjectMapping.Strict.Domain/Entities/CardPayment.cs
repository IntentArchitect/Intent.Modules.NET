using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace ObjectMapping.Strict.Domain.Entities
{
    public class CardPayment : PaymentMethod
    {
        public CardPayment()
        {
            CardLast4 = null!;
        }

        public string CardLast4 { get; set; }
    }
}