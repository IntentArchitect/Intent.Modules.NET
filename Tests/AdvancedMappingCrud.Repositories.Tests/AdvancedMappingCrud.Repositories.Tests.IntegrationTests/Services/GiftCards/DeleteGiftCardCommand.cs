using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.GiftCards
{
    public class DeleteGiftCardCommand
    {
        public DeleteGiftCardCommand()
        {
            CardCode = null!;
        }

        public string CardCode { get; set; }

        public static DeleteGiftCardCommand Create(string cardCode)
        {
            return new DeleteGiftCardCommand
            {
                CardCode = cardCode
            };
        }
    }
}