using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace ValueObjects.Class.Application.TestEntities
{
    public class CreateTestEntityMoneyDto
    {
        public CreateTestEntityMoneyDto()
        {
            Currency = null!;
        }

        public decimal Amount { get; set; }
        public string Currency { get; set; }

        public static CreateTestEntityMoneyDto Create(decimal amount, string currency)
        {
            return new CreateTestEntityMoneyDto
            {
                Amount = amount,
                Currency = currency
            };
        }
    }
}