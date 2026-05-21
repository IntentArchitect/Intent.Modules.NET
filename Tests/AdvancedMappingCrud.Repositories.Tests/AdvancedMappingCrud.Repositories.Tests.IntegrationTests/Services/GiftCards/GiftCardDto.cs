using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.GiftCards
{
    public class GiftCardDto
    {
        public GiftCardDto()
        {
            CardCode = null!;
        }

        public string CardCode { get; set; }
        public decimal Value { get; set; }
        public Guid? UserId { get; set; }

        public static GiftCardDto Create(string cardCode, decimal value, Guid? userId)
        {
            return new GiftCardDto
            {
                CardCode = cardCode,
                Value = value,
                UserId = userId
            };
        }
    }
}