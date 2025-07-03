using AutoFixture;
using IntegrationTesting.Tests.IntegrationTests.HttpClients.HasDateOnlyField.HasDateOnlyFields;
using IntegrationTesting.Tests.IntegrationTests.Services.HasDateOnlyField.HasDateOnlyFields;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Tests.HasDateOnlyFields
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class UpdateHasDateOnlyFieldTests : BaseIntegrationTest
    {
        public UpdateHasDateOnlyFieldTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task UpdateHasDateOnlyField_ShouldUpdateHasDateOnlyField()
        {
            // Arrange
            var client = new HasDateOnlyFieldsHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var hasDateOnlyFieldId = await dataFactory.CreateHasDateOnlyField();

            var command = dataFactory.CreateCommand<UpdateHasDateOnlyFieldCommand>();
            command.Id = hasDateOnlyFieldId;

            // Act
            await client.UpdateHasDateOnlyFieldAsync(hasDateOnlyFieldId, command, TestContext.Current.CancellationToken);

            // Assert
            var hasDateOnlyField = await client.GetHasDateOnlyFieldByIdAsync(hasDateOnlyFieldId, TestContext.Current.CancellationToken);
            Assert.NotNull(hasDateOnlyField);
            Assert.Equal(command.MyDate, hasDateOnlyField.MyDate);
        }
    }
}