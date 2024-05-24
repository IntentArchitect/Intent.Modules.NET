using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace ValueObjects.Class.IntegrationTests.Services.TestEntities
{
    public class CreateTestEntityCommand
    {
        public CreateTestEntityCommand()
        {
            Name = null!;
            Amount = null!;
            Address = null!;
        }

        public string Name { get; set; }
        public CreateTestEntityMoneyDto Amount { get; set; }
        public CreateTestEntityAddressDto Address { get; set; }

        public static CreateTestEntityCommand Create(
            string name,
            CreateTestEntityMoneyDto amount,
            CreateTestEntityAddressDto address)
        {
            return new CreateTestEntityCommand
            {
                Name = name,
                Amount = amount,
                Address = address
            };
        }
    }
}