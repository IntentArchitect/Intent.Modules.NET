using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.GiftCards
{
    public class GetGiftCardByIdQuery
    {
        public GetGiftCardByIdQuery()
        {
            CardCode = null!;
        }

        public string CardCode { get; set; }

        public static GetGiftCardByIdQuery Create(string cardCode)
        {
            return new GetGiftCardByIdQuery
            {
                CardCode = cardCode
            };
        }
    }
}