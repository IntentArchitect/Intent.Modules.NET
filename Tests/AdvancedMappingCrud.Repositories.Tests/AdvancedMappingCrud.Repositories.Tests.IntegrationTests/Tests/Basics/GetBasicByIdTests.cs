using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.HttpClients.Basics;
using AutoFixture;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Tests.Basics
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class GetBasicByIdTests : BaseIntegrationTest
    {
        public GetBasicByIdTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetBasicById_ShouldGetBasicById()
        {
            // Arrange
            var client = new BasicsHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var basicId = await dataFactory.CreateBasic();

            // Act
            var basic = await client.GetBasicByIdAsync(basicId, TestContext.Current.CancellationToken);

            // Assert
            Assert.NotNull(basic);
        }
    }
}