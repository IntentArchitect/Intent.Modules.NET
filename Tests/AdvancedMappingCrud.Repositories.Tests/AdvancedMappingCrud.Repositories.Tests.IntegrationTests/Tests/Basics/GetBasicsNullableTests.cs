using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.HttpClients.Basics;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class GetBasicsNullableTests : BaseIntegrationTest
    {
        public GetBasicsNullableTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetBasicsNullable_ShouldGetBasicsNullable()
        {
            // Arrange
            var client = new BasicsHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            await dataFactory.CreateBasic();

            // Act
            var basics = await client.GetBasicsNullableAsync(1, 10, null);

            // Assert
            Assert.NotNull(basics);
            Assert.True(basics.Data.Count() > 0);
        }

    }
}