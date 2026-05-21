using AdvancedMappingCrud.DbContext.Tests.IntegrationTests.Services.GiftCards;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ProxyServiceContract", Version = "1.0")]

namespace AdvancedMappingCrud.DbContext.Tests.IntegrationTests.HttpClients.GiftCards
{
    public interface IGiftCardsService : IDisposable
    {
        Task<string> CreateGiftCardAsync(string id, CreateGiftCardCommand command, CancellationToken cancellationToken = default);
    }
}