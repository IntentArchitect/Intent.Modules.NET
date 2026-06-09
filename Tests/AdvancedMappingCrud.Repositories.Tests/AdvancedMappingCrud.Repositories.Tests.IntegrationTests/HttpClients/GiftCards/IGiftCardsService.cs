using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.GiftCards;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ProxyServiceContract", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.HttpClients.GiftCards
{
    public interface IGiftCardsService : IDisposable
    {
        Task<string> CreateGiftCardAsync(string cardCode, CreateGiftCardCommand command, CancellationToken cancellationToken = default);
        Task DeleteGiftCardAsync(string cardCode, CancellationToken cancellationToken = default);
        Task<GiftCardDto> GetGiftCardByIdAsync(string cardCode, CancellationToken cancellationToken = default);
    }
}