using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace ValueObjects.Class.IntegrationTests.Services.TestEntities
{
    public class TestEntityMoneyDto
    {
        public TestEntityMoneyDto()
        {
            Currency = null!;
        }

        public decimal Amount { get; set; }
        public string Currency { get; set; }

        public static TestEntityMoneyDto Create(decimal amount, string currency)
        {
            return new TestEntityMoneyDto
            {
                Amount = amount,
                Currency = currency
            };
        }
    }
}