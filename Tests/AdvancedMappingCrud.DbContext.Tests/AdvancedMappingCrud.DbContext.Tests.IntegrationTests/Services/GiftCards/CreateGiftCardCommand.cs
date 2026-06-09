using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.DbContext.Tests.IntegrationTests.Services.GiftCards
{
    public class CreateGiftCardCommand
    {
        public CreateGiftCardCommand()
        {
            Id = null!;
        }

        public string Id { get; set; }
        public decimal Value { get; set; }
        public Guid? CustomerId { get; set; }

        public static CreateGiftCardCommand Create(string id, decimal value, Guid? customerId)
        {
            return new CreateGiftCardCommand
            {
                Id = id,
                Value = value,
                CustomerId = customerId
            };
        }
    }
}