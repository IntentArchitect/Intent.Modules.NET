using AutoFixture;
using IntegrationTesting.Tests.IntegrationTests.HttpClients.HasDateOnlyField.HasDateOnlyFields;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class GetHasDateOnlyFieldByIdTests : BaseIntegrationTest
    {
        public GetHasDateOnlyFieldByIdTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetHasDateOnlyFieldById_ShouldGetHasDateOnlyFieldById()
        {
            // Arrange
            var client = new HasDateOnlyFieldsHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var hasDateOnlyFieldId = await dataFactory.CreateHasDateOnlyField();

            // Act
            var hasDateOnlyField = await client.GetHasDateOnlyFieldByIdAsync(hasDateOnlyFieldId);

            // Assert
            Assert.NotNull(hasDateOnlyField);
        }
    }
}