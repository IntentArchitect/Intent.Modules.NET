using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace ValueObjects.Record.IntegrationTests.Services.TestEntities
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