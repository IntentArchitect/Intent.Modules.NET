using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace ValueObjects.Class.IntegrationTests.Services.TestEntities
{
    public class TestEntityDto
    {
        public TestEntityDto()
        {
            Name = null!;
            Amount = null!;
            Address = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public TestEntityMoneyDto Amount { get; set; }
        public TestEntityAddressDto Address { get; set; }

        public static TestEntityDto Create(Guid id, string name, TestEntityMoneyDto amount, TestEntityAddressDto address)
        {
            return new TestEntityDto
            {
                Id = id,
                Name = name,
                Amount = amount,
                Address = address
            };
        }
    }
}