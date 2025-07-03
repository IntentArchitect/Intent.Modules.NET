using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.HttpClients.Optionals;
using AutoFixture;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Tests.Optionals
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class GetOptionalByIdTests : BaseIntegrationTest
    {
        public GetOptionalByIdTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetOptionalById_ShouldGetOptionalById()
        {
            // Arrange
            var client = new OptionalsHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var optionalId = await dataFactory.CreateOptional();

            // Act
            var optional = await client.GetOptionalByIdAsync(optionalId, TestContext.Current.CancellationToken);

            // Assert
            Assert.NotNull(optional);
        }

        [Fact]
        public async Task GetOptionalById_ShouldReturnNullForInvalidGuid()
        {
            //Arrange
            var client = new OptionalsHttpClient(CreateClient());


            //Act
            var optional = await client.GetOptionalByIdAsync(Guid.Empty);

            //Assert
            Assert.Null(optional);
        }

    }
}