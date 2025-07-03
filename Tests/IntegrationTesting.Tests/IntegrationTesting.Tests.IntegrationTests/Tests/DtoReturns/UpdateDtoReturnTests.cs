using AutoFixture;
using IntegrationTesting.Tests.IntegrationTests.HttpClients;
using IntegrationTesting.Tests.IntegrationTests.HttpClients.DtoReturns;
using IntegrationTesting.Tests.IntegrationTests.Services.DtoReturns;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Tests.DtoReturns
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class UpdateDtoReturnTests : BaseIntegrationTest
    {
        public UpdateDtoReturnTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task UpdateDtoReturn_ShouldUpdateDtoReturn()
        {
            // Arrange
            var client = new DtoReturnsHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var dtoReturnId = await dataFactory.CreateDtoReturn();

            var command = dataFactory.CreateCommand<UpdateDtoReturnCommand>();
            command.Id = dtoReturnId;

            // Act
            await client.UpdateDtoReturnAsync(dtoReturnId, command, TestContext.Current.CancellationToken);

            // Assert
            var dtoReturn = await client.GetDtoReturnByIdAsync(dtoReturnId, TestContext.Current.CancellationToken);
            Assert.NotNull(dtoReturn);
            Assert.Equal(command.Name, dtoReturn.Name);
        }
    }
}