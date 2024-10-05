using AutoFixture;
using IntegrationTesting.Tests.IntegrationTests.HttpClients.HasDateOnlyField.HasDateOnlyFields;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class GetHasDateOnlyFieldsTests : BaseIntegrationTest
    {
        public GetHasDateOnlyFieldsTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetHasDateOnlyFields_ShouldGetHasDateOnlyFields()
        {
            // Arrange
            var client = new HasDateOnlyFieldsHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            await dataFactory.CreateHasDateOnlyField();

            // Act
            var hasDateOnlyFields = await client.GetHasDateOnlyFieldsAsync();

            // Assert
            Assert.True(hasDateOnlyFields.Count > 0);
        }
    }
}