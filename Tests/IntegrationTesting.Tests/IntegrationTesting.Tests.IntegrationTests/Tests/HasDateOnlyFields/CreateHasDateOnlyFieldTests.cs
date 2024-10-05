using AutoFixture;
using IntegrationTesting.Tests.IntegrationTests.HttpClients.HasDateOnlyField.HasDateOnlyFields;
using IntegrationTesting.Tests.IntegrationTests.Services.HasDateOnlyField.HasDateOnlyFields;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class CreateHasDateOnlyFieldTests : BaseIntegrationTest
    {
        public CreateHasDateOnlyFieldTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task CreateHasDateOnlyField_ShouldCreateHasDateOnlyField()
        {
            // Arrange
            var client = new HasDateOnlyFieldsHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);

            var command = dataFactory.CreateCommand<CreateHasDateOnlyFieldCommand>();

            // Act
            var hasDateOnlyFieldId = await client.CreateHasDateOnlyFieldAsync(command);

            // Assert
            var hasDateOnlyField = await client.GetHasDateOnlyFieldByIdAsync(hasDateOnlyFieldId);
            Assert.NotNull(hasDateOnlyField);
        }
    }
}