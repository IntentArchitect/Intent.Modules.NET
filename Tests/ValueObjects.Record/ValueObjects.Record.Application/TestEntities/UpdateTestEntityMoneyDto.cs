using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace ValueObjects.Record.Application.TestEntities
{
    public class UpdateTestEntityMoneyDto
    {
        public UpdateTestEntityMoneyDto()
        {
            Currency = null!;
        }

        public decimal Amount { get; set; }
        public string Currency { get; set; }

        public static UpdateTestEntityMoneyDto Create(decimal amount, string currency)
        {
            return new UpdateTestEntityMoneyDto
            {
                Amount = amount,
                Currency = currency
            };
        }
    }
}