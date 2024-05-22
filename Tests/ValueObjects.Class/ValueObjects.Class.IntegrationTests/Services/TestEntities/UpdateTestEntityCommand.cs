using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace ValueObjects.Class.IntegrationTests.Services.TestEntities
{
    public class UpdateTestEntityCommand
    {
        public UpdateTestEntityCommand()
        {
            Name = null!;
            Amount = null!;
            Address = null!;
        }

        public string Name { get; set; }
        public UpdateTestEntityMoneyDto Amount { get; set; }
        public UpdateTestEntityAddressDto Address { get; set; }
        public Guid Id { get; set; }

        public static UpdateTestEntityCommand Create(
            string name,
            UpdateTestEntityMoneyDto amount,
            UpdateTestEntityAddressDto address,
            Guid id)
        {
            return new UpdateTestEntityCommand
            {
                Name = name,
                Amount = amount,
                Address = address,
                Id = id
            };
        }
    }
}